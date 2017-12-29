using static Newtonsoft.Json.JsonConvert;
using System;

namespace KersDb
{
    public class Thing
    {
        public int Get(int left, int right) =>
            DeserializeObject<int>($"{left + right}");
    }
}
