using System;
using TacticalSentry.Core.Entities;

namespace TacticalSentry.Domain.Rules
{
    public class SessionManager
    {
        private static SessionManager _instance;
        public static SessionManager Instance => _instance ??= new SessionManager();

        public OperatorUser CurrentOperator { get; private set; }
        public DateTime LoginTime { get; private set; }

        public void StartSession(OperatorUser user)
        {
            CurrentOperator = user;
            LoginTime = DateTime.Now;
        }
    }
}