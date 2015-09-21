using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracy
{
    public class TracyFacade
    {
        #region Singleton
        /// <summary>
        /// Get the singleton instance
        /// </summary>
        public static TracyFacade Instance
        {
            get { return SingletonHelper._instance; }
        }

        /// <summary>
        /// Nested helper class
        /// </summary>
        private class SingletonHelper
        {
            static SingletonHelper() { }
            internal static readonly TracyFacade _instance = new TracyFacade();
        }
        private TracyFacade()
        {
            //Pre init codes come here
        }
        #endregion
        private static DataAccess.TracyDB _database = new DataAccess.TracyDB();
        private TracyManager _manager = new TracyManager(_database);
        public TracyManager Manager { get { return _manager; } }

        private UserManager _userManager = new UserManager(_database);
        public UserManager UserManager { get { return _userManager; } }
    }
}
