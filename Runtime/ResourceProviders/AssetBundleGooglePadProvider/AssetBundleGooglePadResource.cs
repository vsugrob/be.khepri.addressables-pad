﻿using Google.Play.AssetDelivery;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Khepri.AssetDelivery.ResourceProviders {
	class AssetBundleGooglePadResource : IAssetBundleResource {
		internal ProvideHandle provideHandle;
        internal AssetBundleRequestOptions reqOptions;
		private long bytesToDownload;
        private AssetBundle retrievedAssetBundle;
        public AssetBundleGooglePadProvider Provider { get; }
        private Dictionary <string, PlayAssetPackRequest> PacksByName => Provider.PacksByName;

        internal AssetBundleGooglePadResource ( AssetBundleGooglePadProvider provider ) {
            Provider = provider;
        }

		internal void Start ( ProvideHandle provideHandle ) {
			this.provideHandle = provideHandle;
			reqOptions = provideHandle.Location.Data as AssetBundleRequestOptions;
            if ( reqOptions != null )
				bytesToDownload = reqOptions.ComputeSize ( provideHandle.Location, provideHandle.ResourceManager );

			provideHandle.SetProgressCallback ( PercentComplete );
            provideHandle.SetDownloadProgressCallbacks ( GetDownloadStatus );
            BeginOperation ();
		}

		private void BeginOperation () {
			var path = provideHandle.ResourceManager.TransformInternalId ( provideHandle.Location );
            // TODO: get pack name by path (from some resource table).
            BeginOperation ( AssetPackBundleConfig.InstallTimePackName, path );
		}

        private void BeginOperation ( string packName, string path ) {
            UnityEngine.Debug.Log ( $"AssetBundleGooglePadResource.BeginOperation: {packName}, {path}" );   // TODO: remove.
            // Google PAD API doesn't like to receive parallel requests for the same asset pack.
            if ( !PacksByName.TryGetValue ( packName, out var packRequest ) ) {
                packRequest = PlayAssetDelivery.RetrieveAssetPackAsync ( packName );
                PacksByName [packName] = packRequest;
            }

			packRequest.Completed += PackRequest_Completed;
            void PackRequest_Completed ( PlayAssetPackRequest pr ) {
                packRequest.Completed -= PackRequest_Completed;
                if ( packRequest.Error != AssetDeliveryErrorCode.NoError ) {
                    UnityEngine.Debug.LogError ( $"Error loading asset pack {packName}: {packRequest.Error}" );
                    return;
                }

			    var bundleRequest = packRequest.LoadAssetBundleAsync ( path );
				bundleRequest.completed += BundleRequest_completed;
		    }
		}

		private void BundleRequest_completed ( AsyncOperation op ) {
			retrievedAssetBundle = ( op as AssetBundleCreateRequest )?.assetBundle;
            provideHandle.Complete ( this, retrievedAssetBundle != null, null );
		}

        public AssetBundle GetAssetBundle () => retrievedAssetBundle;

		// TODO: implement.
		private float PercentComplete () => 1;
		private DownloadStatus GetDownloadStatus () {
            if ( reqOptions == null ) return default;
            var status = new DownloadStatus () {
                TotalBytes = bytesToDownload,
                DownloadedBytes = bytesToDownload,
                IsDone = PercentComplete () >= 1f
            };

            return status;
        }
	}
}