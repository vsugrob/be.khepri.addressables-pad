using Google.Play.AssetDelivery;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Khepri.AssetDelivery.ResourceProviders {
	[DisplayName ( "Async Asset Pack Bundle Provider" )]
	public class AssetBundleGooglePadProvider : AssetBundleProvider {
		public bool UseGooglePad => AndroidBuildTypeConfig.Singleton.IsAabBuild &&
			#if UNITY_ANDROID && !UNITY_EDITOR
			true;
			#else
			false;
			#endif
		internal Dictionary <string, PlayAssetPackRequest> PacksByName { get; } =
			 new Dictionary <string, PlayAssetPackRequest> ();

		/// <inheritdoc/>
        public override void Provide ( ProvideHandle providerInterface ) {
			if ( UseGooglePad )
				new AssetBundleGooglePadResource ( this ).Start ( providerInterface );
			else
				base.Provide ( providerInterface );
        }
	}
}
