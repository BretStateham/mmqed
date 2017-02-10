using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PiBrainBasic.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace PiBrainBasic.Services
{
  public static class CameraService
  {
    private static object cameraLock = new object();

    public static DeviceInformationCollection Cameras { get; set; }

    private static string azureStorageAccount = "udlstorage";
    private static string azureStoragePrimaryKey = "4+8mxiuVFKPq2vCci8A9oV49+DXpXzr5A8d7BlaZ1xz7xBCm3Cnf8bt0joo+TgJ3uSimiu298QIAwyStNAtUWg==";
    private static string azureStorageContainer = "cameracaptures";

    static CameraService()
    {
    }

    public static async Task FindCameras()
    {
      // Get available devices for capturing pictures
      Cameras = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

      foreach (DeviceInformation camera in Cameras)
      {
        Debug.WriteLine(string.Format("{0} - {1} - {2}", camera.Id, camera.Name, camera.Kind));
      }
    }

    private static string BuildFileName(TakePictureCommandEventArgs e)
    {
      return String.Format("{0}-{1}.jpg", e.CommandMessage.Team, e.Message.MessageId);
    }

    public async static Task<String> ProcessTakePictureCommand(TakePictureCommandEventArgs e)
    {

      string photoFileName = "";

      try
      {

        

        DeviceInformation camera = Cameras[e.Camera];

        MediaCapture mediaCapture = new MediaCapture();

        MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings { VideoDeviceId = camera.Id, StreamingCaptureMode = StreamingCaptureMode.Video };

        await mediaCapture.InitializeAsync(settings);

        // Photo StorageFile.
        StorageFile photoFile;

        // Photo file name.
        //string photoFileName = buildDateTimeStamp() + ".jpg";
        photoFileName = BuildFileName(e);

        // Create the PhotoFile in the PicturesLibrary System folder.
        photoFile = await KnownFolders.PicturesLibrary.CreateFileAsync(photoFileName, CreationCollisionOption.ReplaceExisting);

        // Create an ImageEncodingProperties object for the photo file.
        ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();

        // Using the MediaCapture, capture the photo and save it to the PicturesLibrary System folder.
        await mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, photoFile);

        // Create an Azure CloudStorageAccount object.
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=" + azureStorageAccount + ";AccountKey=" + azureStoragePrimaryKey);

        // Create a CloudBlobClient object using the CloudStorageAccount.
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        // Create a CloudBlobContain object using the CloudBlobClient.
        CloudBlobContainer container = blobClient.GetContainerReference(azureStorageContainer);

        //  Create a CloudBlockBlob using the CloudBlobContainer.
        CloudBlockBlob blockBlob = container.GetBlockBlobReference(photoFileName);

        //  Using the CloudBlockBlob, Upload the file to Azure.
        await blockBlob.UploadFromFileAsync(photoFile);

        // Clean up the media capture
        mediaCapture.Dispose();

        mediaCapture = null;

        

      }
      catch (Exception ex)
      {
        throw(ex);
      }

      return photoFileName;

      //  catch (UnauthorizedAccessException)
      //  {
      //    // This will be thrown if the user denied access to the camera in privacy settings
      //    System.Diagnostics.Debug.WriteLine("The app was denied access to the camera");
      //  }
      //  catch (Exception ex)
      //  {
      //    System.Diagnostics.Debug.WriteLine("MediaCapture initialization failed. {0}", ex.Message);
      //  }
      //}
    }

    public async static Task<String> ProcessTakePictureCommandFixed(TakePictureCommandEventArgs e)
    {

      string photoFileName = "";

      try
      {



        DeviceInformation camera = Cameras[e.Camera];

        //MediaCapture mediaCapture = new MediaCapture();

        //MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings { VideoDeviceId = camera.Id, StreamingCaptureMode = StreamingCaptureMode.Video };

        //await mediaCapture.InitializeAsync(settings);

        // Photo StorageFile.
        //StorageFile photoFile;
        StorageFile localPhotoFile;

        // Photo file name.
        //string photoFileName = buildDateTimeStamp() + ".jpg";
        photoFileName = BuildFileName(e);

        string photoLocalName = string.Format("camera{0}.jpg", e.Camera);
        
        StorageFolder picsLibrary = KnownFolders.PicturesLibrary;

        localPhotoFile  = await picsLibrary.GetFileAsync(photoLocalName);

        // Create the PhotoFile in the PicturesLibrary System folder.
        //photoFile = await KnownFolders.PicturesLibrary.CreateFileAsync(photoLocalName, CreationCollisionOption.ReplaceExisting);

        // Create an ImageEncodingProperties object for the photo file.
        //ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();

        // Using the MediaCapture, capture the photo and save it to the PicturesLibrary System folder.
        //await mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, photoFile);

        // Create an Azure CloudStorageAccount object.
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=" + azureStorageAccount + ";AccountKey=" + azureStoragePrimaryKey);

        // Create a CloudBlobClient object using the CloudStorageAccount.
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        // Create a CloudBlobContain object using the CloudBlobClient.
        CloudBlobContainer container = blobClient.GetContainerReference(azureStorageContainer);

        //  Create a CloudBlockBlob using the CloudBlobContainer.
        CloudBlockBlob blockBlob = container.GetBlockBlobReference(photoFileName);

        //  Using the CloudBlockBlob, Upload the file to Azure.
        await blockBlob.UploadFromFileAsync(localPhotoFile);

        // Clean up the media capture
        //mediaCapture.Dispose();

        //mediaCapture = null;



      }
      catch (Exception ex)
      {
        throw (ex);
      }

      return photoFileName;

      //  catch (UnauthorizedAccessException)
      //  {
      //    // This will be thrown if the user denied access to the camera in privacy settings
      //    System.Diagnostics.Debug.WriteLine("The app was denied access to the camera");
      //  }
      //  catch (Exception ex)
      //  {
      //    System.Diagnostics.Debug.WriteLine("MediaCapture initialization failed. {0}", ex.Message);
      //  }
      //}
    }


  }
}
