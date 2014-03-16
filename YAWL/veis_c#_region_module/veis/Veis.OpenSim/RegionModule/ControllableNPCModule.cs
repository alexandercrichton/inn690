using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using Mono.Addins;
using Nini.Config;
using log4net;
using OpenSim.Region.Framework.Scenes;
using OpenSim.Region.Framework.Interfaces;
using OpenSim.Region.OptionalModules.World.NPC;
using OpenSim.Region.ScriptEngine.Shared.Api;
using OpenSim.Framework;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using Veis.Simulation;
using Veis.OpenSim.Simulation;
using Veis.WebInterface;

[assembly: Addin("ControllableNPCModule", "0.1")]
[assembly: AddinDependency("OpenSim", "0.5")]

namespace Veis.OpenSim.RegionModule
{
    public delegate bool LaunchCaseEventHandler(string specificationName);
    public delegate bool CancelCaseEventHandler(string specifcationName, int? caseNumber);
    public delegate void SimulationEventHandler(SimulationActions action);
    public delegate void ChatEventHandler(OSChatMessage message);
    public delegate void RequestDataEventHandler(RequestDataEventArgs e);
    public delegate void UserEventHandler(UserArgs e);
    
    /// <summary>
    /// The purpose of this module is to act as a wrapper for the internal NPC Module.
    /// When the simulation requests the creation of an NPC, it will call the respective NPCModule function.
    /// The new NPC UUID will both be stored internally in OpenSim, and locally, in our own controllable NPC
    /// class. Because OpenSim works on "ScenePresence" based on UUID, it is all we need to store to control
    /// the avatars locally. 
    /// </summary>
    [Extension(Path="/OpenSim/RegionModules", NodeName="RegionModule", Id="ControllableNPCModule")]
    public class ControllableNPCModule : INonSharedRegionModule
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private Scene _scene;                           // A reference to the current scene
        private NPCModule _npcModule;                   // A reference to OpenSim's NPC Module
        private IScriptModuleComms _commsMod;           // A reference to the scripting module to interpret script commands
        
        private Veis.Simulation.Simulation _simulation;      // The simulation this module is running
        private Dictionary<string, AvatarAppearance> _appearanceCache = new Dictionary<string, AvatarAppearance>();
        private Dictionary<UUID, NPCAvatar> _createdAvatars = new Dictionary<UUID, NPCAvatar>(); // List of avatars that have been created by this module

        public const string ScriptModuleName = "CRCMOD";

        #region Region Module Methods

        public string Name { get { return "Controllable NPC Module"; } }

        public Type ReplaceableInterface { get { return null; } }

        public void Initialise(IConfigSource source)
        {
            string name = source.Configs[0].Name;
            //_log.Error("[Fortune]: " + name); //It shows "[Fortune]: Startup"

            if (source.Configs[name] == null)
            {
                source.AddConfig(name);
            }

            // Retrieve the UUID of the NPCBox from the region config, if there is one 
            // IConfig config = source.Configs[name];
            // OpenSimUtilities.NPCBox = UUID.Parse(config.GetString("NPCBoxUUID", UUID.Zero.ToString()));
                       
            _log.DebugFormat("[NPC Controller]: INITIALIZED MODULE");
        }


        public void Close()
        {
            _log.DebugFormat("[NPC Controller]: CLOSED MODULE");
        }

        public void AddRegion(Scene scene)
        {
            // Store a reference to the scene for future reference
            // Does it needs to be stored in an array, since the game world now is more than 1 region?
            this._scene = scene;
            _log.DebugFormat("[NPC Controller]: REGION {0} ADDED", scene.RegionInfo.RegionName);
        }


        public void RemoveRegion(Scene scene)
        {
            _simulation.End();
            // Remove all avatars (if not already)
            RemoveAll();

            _log.DebugFormat("[NPC Controller]: REGION {0} REMOVED", scene.RegionInfo.RegionName);
        }


        /// <summary>
        /// This method is called after all modules have been loaded for a region. This is where
        /// the reference to NPCModule should be gathered. In addition, this will be the start off point for
        /// out simulation hooks
        /// </summary>
        /// <param name="scene"></param>
        public void RegionLoaded(Scene scene)
        {
            // Retrieve reference to the NPCModule of Opensim
            _npcModule = scene.Modules["NPCModule"] as NPCModule;

            // Retrieve a reference to the scripting module
            _commsMod = scene.RequestModuleInterface<IScriptModuleComms>();
            _commsMod.OnScriptCommand += new ScriptCommand(ProcessScriptCommand);

            // Create the simulation classes
            _simulation = new OpenSimYAWLSimulation(this);

            // Make sure this module is notified when a new client is added, so 
            // our own chat handler can be added
            scene.EventManager.OnChatFromClient += new EventManager.ChatFromClientEvent(EventManager_OnChatFromClient);

            // We need to know when a new client is added so we can check if it can receive tasks
            scene.EventManager.OnClientLogin += new Action<IClientAPI>(EventManager_OnClientLogin);
            scene.EventManager.OnClientClosed += new EventManager.ClientClosed(EventManager_OnClientClosed);

            _log.DebugFormat("[NPC Controller]: REGION {0} LOADED", scene.RegionInfo.RegionName);
        }

        public Scene GetScene()
        {
            return _scene;
        }

        #endregion

        #region Script Commands

        public void ProcessScriptCommand(UUID script, string id, string module, string command, string k)
        {
            //If the script command are not "CRCMOD", then ignore it.
            if (ScriptModuleName != module) return;

            string[] tokens  = command.Split( new char[] { '|' }, StringSplitOptions.None);

            string function = tokens[0];
            switch (function)
            {
                    
                case "LaunchCase":
                    //CRCMOD|LaunchCase|SpecificationName
                    string name = tokens[1];
                    bool isOK = OnLaunchCase(name);
                    string msg = "Launched case successfully. [";
                    if (!isOK) msg = "Launched case failed. [";
                    
                    _commsMod.DispatchReply(script, 1, msg + name + "]", "");
                    break;
                
                case "EndAllCases":
                    //CRCMOD|EndAllCases
                    OnCancelCase("", (int?)null);
                    _commsMod.DispatchReply(script, 1, "Attempting to cancel all running cases", "");
                    break; 
                
                case "Simulation": // Performs the given simulation action on listening simulators
                    if (tokens.Length > 1)
                    {
                        if (tokens[1] == "Reset")
                        {
                            OnSimulationAction(SimulationActions.Reset);
                            _commsMod.DispatchReply(script, 1, "Attempting to reset simulation", "");
                        }
                        else if (tokens[1] == "Start")
                        {
                            OnSimulationAction(SimulationActions.Start);
                            _commsMod.DispatchReply(script, 1, "Initialising simulation", "");
                        }
                    }
                    else
                    {
                        _commsMod.DispatchReply(script, 1, "Missing argument for Simulation modCommand", "");
                    }
                    break;
                
                case "GetAll": // Gets all information of a given type string
                    if (tokens.Length > 1)
                    {
                        OnDataRequested(new RequestDataEventArgs { DataType = tokens[1], DataFilter = new Dictionary<string, object>(), Destination = script.ToString()});
                    }
                    break;
                
                case "Get": // Gets specific information of a given type, with a filter. Filter string is of format <Field>:<Value>+<Field>:<Value>+ ..etc
                    if (tokens.Length > 2)
                    {
                        var dataFilter = new Dictionary<string, object>();
                        string[] filterTokens = tokens[2].Split(new char[] {':', '+' }, StringSplitOptions.None);
                        for (int i = 0; i < filterTokens.Length; i += 2)
                        {
                            dataFilter.Add(filterTokens[i], filterTokens[i+1]);
                        }

                        OnDataRequested(new RequestDataEventArgs { DataType = tokens[1], DataFilter = dataFilter, Destination = script.ToString() });
                    }
                    break;
                
                case "RegisterUser": // Involves the given user in the simulation
                    if (tokens.Length > 2)
                    {
                        OnRegisterUser(new UserArgs
                        { RoleName = tokens[1], UserId = tokens[2] });
                    }
                    break;
            }
        }

        #endregion

        #region Events

        public event LaunchCaseEventHandler LaunchCase;
        protected virtual bool OnLaunchCase(string specificationName)
        {
            if (LaunchCase != null)
            {
                return LaunchCase(specificationName);
            }
            return false;
        }

        public event CancelCaseEventHandler CancelCase;
        protected virtual bool OnCancelCase(string specificationName, int? caseNumber)
        {
            if (CancelCase != null)
            {
                return CancelCase(specificationName, caseNumber);
            }
            return false;
        }

        public event SimulationEventHandler SimulationAction;
        protected virtual void OnSimulationAction(SimulationActions action)
        {
            if (SimulationAction != null) SimulationAction(action);
        }

        public event ChatEventHandler ChatSentByClient;
        protected virtual void OnChatSentByClient(OSChatMessage message)
        {
            if (ChatSentByClient != null)
                ChatSentByClient(message);
        }

        void EventManager_OnChatFromClient(object sender, OSChatMessage chat)
        {
            if (chat.Channel == 0 && (chat.Type == ChatTypeEnum.Say || chat.Type == ChatTypeEnum.Shout || chat.Type == ChatTypeEnum.Whisper))
            {
                if (!_createdAvatars.Keys.Contains(chat.SenderUUID))
                    OnChatSentByClient(chat);
            }
        }

        public event RequestDataEventHandler DataRequested;
        protected virtual void OnDataRequested(RequestDataEventArgs e)
        {
            if (DataRequested != null)
                DataRequested(e);
        }

        public event UserEventHandler RegisterUser;
        protected virtual void OnRegisterUser(UserArgs e)
        {
            if (RegisterUser != null)
                RegisterUser(e);
        }


        void EventManager_OnClientLogin(IClientAPI obj)
        {
            // TODO: Notify simulation
        }


        void EventManager_OnClientClosed(UUID clientID, Scene scene)
        {
            // TODO: Raise event
        }

        #endregion

        #region NPC Simulation Methods

        /// <summary>
        /// Creates a new NPC for this simulation, and creates it via the NPC module also.
        /// </summary>
        /// <returns>The UUID of the new NPC</returns>
        public UUID CreateNPC(string firstname,
            string lastname,
            Vector3 position,
            AvatarAppearance appearance)
        {
            UUID newNPC = _npcModule.CreateNPC(firstname, lastname, position, UUID.Zero, true, _scene, appearance);
            if (newNPC != UUID.Zero)
            {
                NPCAvatar internalAvatar = _npcModule.GetNPC(newNPC, _scene) as NPCAvatar;
                _createdAvatars.Add(newNPC, internalAvatar);
                internalAvatar.OnStopMovement += delegate { Log("OnStopMovement" + internalAvatar.Name); };
                internalAvatar.OnStopAnim += delegate { Log("OnStopAnim" + internalAvatar.Name); };
            }
            return newNPC;
        }

        /// <summary>
        /// Returns the NPC with the give UUID if created by this module
        /// </summary>
        /// <returns>Null if not found</returns>
        public NPCAvatar GetNPC(UUID npcUUID)
        {
            if (_createdAvatars.ContainsKey(npcUUID))
            {
                return _createdAvatars[npcUUID];
            }
            return null;
        }

        /// <summary>
        /// Removes all NPC from the region
        /// </summary>
        public void RemoveAll()
        {
            lock (_createdAvatars)
            {
                UUID[] keys = new UUID[_createdAvatars.Count];
                _createdAvatars.Keys.CopyTo(keys, 0);

                foreach (UUID key in keys)
                {
                    _npcModule.DeleteNPC(key, _scene);
                }

                _createdAvatars.Clear();
            }
        }

        /// <summary>
        /// Removes the NPC with the given key from the region
        /// </summary>
        public void RemoveNPC(UUID avatarUUID)
        {
            lock (_createdAvatars)
            {
                _npcModule.DeleteNPC(avatarUUID, _scene);
                _createdAvatars.Remove(avatarUUID);
            }
        }

        /// <summary>
        /// Gets the starting position for any NPCs spawned. They spawn near the location
        /// of an object called "NPCBox" (defined in OpenSimUtilities). If that object does
        /// not exist, they spawn in a hard-coded location on the map.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetDefaultStartingPosition()
        {
            Vector3 spawnPos = GetPositionNearObject(OpenSimUtilities.NPCSpawnPointName, 1.0f);
            if (spawnPos == Vector3.Zero)
            {
                spawnPos = OpenSimUtilities.DefaultSpawnPoint;
            }
            return spawnPos;
        }

        public Vector3 GetPositionNearObject(string objName, float distance)
        {
            SceneObjectGroup obj = _scene.GetSceneObjectGroup(objName);
            if (obj != null)
            {
                return obj.AbsolutePosition + Vector3.UnitZ * distance;
            }

            return Vector3.Zero;
        }



        /// <summary>
        /// Finds an appearance notecard by name, reads it, then returns it.
        /// </summary>
        /// <param name="appearanceName">The name of the appearance</param>
        /// <returns>An AvatarAppearance of the given name</returns>
        internal AvatarAppearance GetAppearance(string appearanceName)
        {
            AvatarAppearance appearance = null;

            if (_appearanceCache.ContainsKey(appearanceName))
            {
                appearance = _appearanceCache[appearanceName];
            }
            else
            {
                UUID notecard = UUID.Zero;

                SceneObjectPart sop = _scene.GetSceneObjectPart(OpenSimUtilities.NPCBoxName);
                if (sop == null) return null;
                IList<TaskInventoryItem> items = sop.Inventory.GetInventoryItems(appearanceName);

                if (items.Count == 1)
                {
                    notecard = items[0].AssetID;
                }
                else
                {
                    return null;
                }

                StringBuilder notecardData = new StringBuilder();

                AssetBase a = _scene.AssetService.Get(notecard.ToString());

                if (a == null)
                    return null;

                System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
                string data = enc.GetString(a.Data);
                NotecardCache.Cache(notecard, data);

                for (int count = 0; count < NotecardCache.GetLines(notecard); count++)
                {
                    string line = NotecardCache.GetLine(notecard, count) + "\n";

                    _log.DebugFormat("[OSSL]: From notecard {0} loading line {1}", notecard, line);

                    notecardData.Append(line);
                }

                string appearanceSerialized = notecardData.ToString();

                if (appearanceSerialized == "") appearanceSerialized = null;

                if (appearanceSerialized != null)
                {
                    OSDMap appearanceOsd = (OSDMap)OSDParser.DeserializeLLSDXml(appearanceSerialized);
                    appearance = new AvatarAppearance();
                    appearance.Unpack(appearanceOsd);

                    _appearanceCache[appearanceName] = appearance;
                }
            }

            return appearance;
        }

        public void Log(string message)
        {
            _log.DebugFormat("[NPC Controller]: " + message);
        }

        public void SendData(List<string> dataList, bool applyLongListFormatting, UUID destination)
        {
            const int MaxShortLength = 8;
            StringBuilder sb = new StringBuilder();
            if (dataList.Count() <= MaxShortLength)
            {
                sb.Append("SHORT");
                foreach (string data in dataList)
                {
                    sb.AppendFormat("|{0}", data);
                }
            }
            else if (dataList.Count() > MaxShortLength && applyLongListFormatting)
            {
                sb.Append("LONG|");
                const string Separator = ", ";
                foreach (string data in dataList)
                {
                    sb.Append(data);
                    sb.Append(Separator);
                }
                sb.Remove(sb.Length - Separator.Length, Separator.Length);
            }
            else
            {
                sb.Append("LONG");
                foreach (string data in dataList)
                {
                    sb.AppendFormat("|{0}", data);
                }
            }

            _commsMod.DispatchReply(destination, 1, sb.ToString(), "");
        }

        public void SendData(string data, UUID destination)
        {
            _commsMod.DispatchReply(destination, 1, data, "");
        }

        // This is currently wrong - needs to be dealt with
        //public void LoadAgentsFromFile()
        //{
        //    if (Directory.Exists(@"agents\\" + scene.RegionInfo.RegionName))
        //    {
        //        string[] agents = Directory.GetFiles(@"agents\\" + scene.RegionInfo.RegionName);

        //        foreach (string agentFilename in agents)
        //        {
        //            string[] lines = File.ReadAllLines(agentFilename, Encoding.ASCII);
        //            System.Collections.ArrayList commands = new System.Collections.ArrayList();
        //            if (lines.Length > 2)
        //            {
        //                string npcName = lines[0];
        //                string npcLocation = lines[1];
        //                string npcAppearance = lines[2];
        //                for (int i = 3; i < lines.Length; i++)
        //                {
        //                    commands.Add(lines[i]);
        //                }

        //                AvatarAppearance appearance = GetAppearance(npcAppearance);

        //                if (appearance == null)
        //                    return;

        //                UUID npcKey = CreateNPC(npcName.Split(' ')[0], npcName.Split(' ')[1], Vector3.Parse(npcLocation), this.scene, appearance);
        //                this.m_avatars[npcKey].AddTaskToQueue("WAIT:15"); //Just so the other tasks have time to enter...
        //                foreach (string task in commands)
        //                {
        //                    this.m_avatars[npcKey].AddTaskToQueue(task);
        //                }
        //            }
        //        }
        //    }
        //}

        #endregion
    }
}
