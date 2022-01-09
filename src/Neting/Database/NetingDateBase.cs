using dotnet_etcd;

namespace Neting.Database
{
    public class NetingDateBase
    {
        private readonly EtcdClient _etcdClient;

        public NetingDateBase(EtcdClient etcdClient)
        {
            _etcdClient = etcdClient;
            NetingCluster = new NetingDbSet<NetingCluster>(Constants.NetingClusterPath, _etcdClient);
            NetingRoute = new NetingDbSet<NetingRoute>(Constants.NetingRoutePath, _etcdClient);
        }

        public DbSet<NetingCluster> NetingCluster { get; private init; }
        public DbSet<NetingRoute> NetingRoute { get; private init; }

        private class NetingDbSet<T> : DbSet<T> where T : class
        {
            public NetingDbSet(string path, EtcdClient etcdClient) : base(etcdClient)
            {
                _key = path;
            }

            private readonly string _key;
            public override string KeyPrefix => _key;
        }
    }
}
