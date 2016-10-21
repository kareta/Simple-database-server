
namespace DatabaseServer
{
    class ClientPacket
    {
        public string Query { get; set; }

        public ClientPacket(string data)
        {
            Query = data;
        }

        public override string ToString()
        {
            return Query;
        }
    }
}
