﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Data;
using Veis.Data.Entities;
using System.Timers;
using UnityEngine;

namespace Veis.Simulation.WorldState.StateSources
{
    public class PolledDatabaseStateSource : IStateSource
    {
        private readonly IRepository<Data.Entities.WorldState> _worldStateRepository;
        private readonly IRepository<AccessRecord> _accessRecordRepository;
        private DateTime _lastChecked;
        private Timer _poll;


        public PolledDatabaseStateSource(float pollInterval,
            IRepository<Data.Entities.WorldState> worldStateRepos, 
            IRepository<AccessRecord> accessRecordRepos)
        {
            _worldStateRepository = worldStateRepos;
            _accessRecordRepository = accessRecordRepos;
            _poll = new Timer(pollInterval);
            _poll.AutoReset = true;
            _poll.Elapsed += CheckForUpdates;
        }

        public void Start()
        {
            _poll.Start();
        }

        public void Stop()
        {
            _poll.Stop();
        }

        private void CheckForUpdates(object sender, ElapsedEventArgs e)
        {
            try
            {
                AccessRecord lastAccess = _accessRecordRepository.Find().FirstOrDefault();

                // If changes were made since the last time
                if (lastAccess != null && lastAccess.LastUpdated > _lastChecked)
                {
                    OnStateUpdated();
                }
                // Update the time the database was last checked
                _lastChecked = DateTime.Now;
            }
            catch (Exception exception)
            {
                Veis.Data.Logging.Logger.BroadcastMessage(this, exception.Message);
            }
        }

        public List<State> GetAll()
        {
            return _worldStateRepository.Find().Select(ws => new State
            {
                Asset = ws.AssetName,
                Predicate = ws.PredicateLabel,
                Value = ws.Value
            }).ToList();
        }

        public List<State> Get(Func<State, bool> selector)
        {
            return GetAll().Where(selector).ToList();
        }

        private void OnStateUpdated()
        {
            if (StateUpdated != null)
                StateUpdated();
        }

        public event StateUpdatedHandler StateUpdated;
    }
}
