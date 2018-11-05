using Newtonsoft.Json;

namespace ModniteServer.API.Controllers
{
    public sealed class StorefrontController : Controller
    {
        /// <summary>
        /// Gets the keys needed to decrypt some assets.
        /// </summary>
        [Route("GET", "/fortnite/api/storefront/v2/keychain")]
        public void GetKeychain()
        {
            // NOTE: This API gives the encryption key needed to decrypt data for pakchunk1000+

            var response = new[]
            {
                // [GUID]:[base64 encoded key]:[skin]

                // TODO: 0xdc434988002105581bae813d59b92358ae58a8bf2fcc9d2c14a85a75129c4239 CID_255_Athena_Commando_F_HalloweenBunny
                "D8FDE5644FE474C7C3D476AA18426FEB:orzs/Wp9XxE5vpybtd0tOxX6hrMyZZheFZusAw1c+6A=:BID_126_DarkBomber",
                "8F6ACF5D43BC4BC272D72EBC072BDB4F:rsT5K8O82gjB/BWAR7zl6cBstk0xxiu/E0AK/RQNUjE=:CID_246_Athena_Commando_F_Grave",
                "4A8216304A1A18CB9583BC8CFF99EE26:QF3nHCFt1vhELoU4q1VKTmpxnk20c2iAiBEBzlbzQAY=:CID_184_Athena_Commando_M_DurrburgerWorker",
                "FE0FA56F4B280D2F0CB2AB899C645F3E:hYi0DrAf6wtw7Zi+PlUi7/vIlIB3psBzEb5piGLEW6s=:CID_220_Athena_Commando_F_Clown",
                "D50ABA0F48BD66E4044616BDC40F4AD6:GNzmjA0ytPrD6J//HSbVF0qypflabpJ3guKTdX4ZStE=:BID_115_DieselpunkMale",
                "7FA4F2374FFE075000BC209360056A5A:nywIiZlIL8AIMkwCZfrYoAkpHM3zCwddhfszh++6ejI=:CID_223_Athena_Commando_M_Dieselpunk",
                "E45BD1CD4B6669367E57AFB5DC2B4478:EXfrtfslMES/Z2M/wWCEYeQoWzI1GTRaElXhaHBw8YM=:CID_229_Athena_Commando_F_DarkBomber"
            };

            Response.StatusCode = 200;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(response));
        }
    }
}