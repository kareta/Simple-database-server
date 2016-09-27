
namespace DatabaseServer
{
    class ClientPacket
    {
        public string Query { get; set; }

        public ClientPacket(string query)
        {
            Query = query;
        }

        public override string ToString()
        {
            return Query;
        }
    }
}
