using Data;
using CoreGamePlay;

namespace Service
{
    public class UserDataManager
    {
        private static UserDataManager _instance;
        public static UserDataManager Instance => _instance ??= new UserDataManager();

        public UserData UserData { get; private set; }

        private UserDataManager()
        {
            UserData = new UserData();

            // Khởi tạo mặc định nếu cần
            for (int i = 0; i < 3; i++)
            {
                UserData.Slots.Add(new FarmSlot());
            }

            for (int i = 0; i < 1; i++)
            {
                UserData.Workers.Add(new GeneralWorker());
            }

            // Inventory mẫu
            UserData.Inventory["TomatoSeed"] = 10;
            UserData.Inventory["BlueberrySeed"] = 10;
            UserData.Inventory["Milk"] = 0;
        }
    }
}