﻿using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.MiddleTier;
using Xpand.ExpressApp.Model;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Xpo.DB;

namespace Xpand.ExpressApp.Core {
    public static class XafApplicationExtensions {
        public static T FindModule<T>(this XafApplication xafApplication) where T : ModuleBase {
            return (T)xafApplication.Modules.FindModule(typeof(T));
        }

        public static ClientSideSecurity? ClientSideSecurity(this XafApplication xafApplication) {
            var modelOptionsClientSideSecurity = xafApplication.Model.Options as IModelOptionsClientSideSecurity;
            return modelOptionsClientSideSecurity != null ? (modelOptionsClientSideSecurity).ClientSideSecurity : null;
        }

        public static SimpleDataLayer CreateCachedDataLayer(this XafApplication xafApplication, IDataStore argsDataStore) {
            var cacheRoot = new DataCacheRoot(argsDataStore);
            var cacheNode = new DataCacheNode(cacheRoot);
            return new SimpleDataLayer(XpandModuleBase.Dictiorary, cacheNode);
        }

        public static string GetConnectionString(this XafApplication xafApplication) {
            if (xafApplication is ServerApplication && !(xafApplication is IXafApplication))
                throw new NotImplementedException("Use " + typeof(XpandServerApplication) + " insted of " + xafApplication.GetType());
            return ((IConnectionString)xafApplication).ConnectionString;
        }

        public static void CreateCustomObjectSpaceprovider(this XafApplication xafApplication, CreateCustomObjectSpaceProviderEventArgs args) {
            var connectionString = getConnectionStringWithOutThreadSafeDataLayerInitialization(args);
            ((IConnectionString)xafApplication).ConnectionString = connectionString;
            var connectionProvider = XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.DatabaseAndSchema);
            args.ObjectSpaceProvider = ObjectSpaceProvider(xafApplication, connectionProvider, connectionString);
        }

        public static void CreateCustomObjectSpaceprovider(this XafApplication xafApplication, CreateCustomObjectSpaceProviderEventArgs args, string dataStoreNameSuffix) {
            if (DataStoreManager.GetDataStoreAttributes(dataStoreNameSuffix).Any()) {
                xafApplication.CreateCustomObjectSpaceprovider(args);
            }
        }

        static IObjectSpaceProvider ObjectSpaceProvider(XafApplication xafApplication, IDataStore connectionProvider, string connectionString) {
            var xafApplicationDataStore = xafApplication as IXafApplicationDataStore;
            var selectDataSecurityProvider = xafApplication.Security as ISelectDataSecurityProvider;
            if (xafApplicationDataStore != null) {
                IDataStore dataStore = xafApplicationDataStore.GetDataStore(connectionProvider);
                return dataStore != null ? new XpandObjectSpaceProvider(new MultiDataStoreProvider(dataStore), selectDataSecurityProvider)
                                          : new XpandObjectSpaceProvider(new MultiDataStoreProvider(connectionString), selectDataSecurityProvider);
            }
            return new XpandObjectSpaceProvider(new MultiDataStoreProvider(connectionString), selectDataSecurityProvider);
        }

        static string getConnectionStringWithOutThreadSafeDataLayerInitialization(CreateCustomObjectSpaceProviderEventArgs args) {
            return args.Connection != null ? args.Connection.ConnectionString : args.ConnectionString;
        }
    }
}