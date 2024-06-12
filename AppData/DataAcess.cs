using MVP_DashBoard.Models;

namespace MVP_DashBoard.AppData
{
    public class DataAcess
    {
        public List<ReceptionData>? receptionMongoResult
        {
            get; set;
        }
        public List<DispatchData>? dispatchMongoResult
        {
            get; set;
        }

    }
}
