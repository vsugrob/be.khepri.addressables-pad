using UnityEngine;

namespace Khepri.AssetDelivery {
    public class AndroidBuildTypeConfig : ScriptableObject {
        public const string FileName = "AndroidBuildType";
        public const string Path = "Assets/Resources/" + FileName + ".asset";
        public bool IsAabBuild;
        private static AndroidBuildTypeConfig _singleton;
        public static AndroidBuildTypeConfig Singleton {
            get {
                if ( _singleton == null ) {
                    _singleton = Resources.Load <AndroidBuildTypeConfig> ( FileName );
                    if ( _singleton == null )
                        _singleton = CreateInstance <AndroidBuildTypeConfig> ();
                }

                return _singleton;
            }
        }
    }
}
