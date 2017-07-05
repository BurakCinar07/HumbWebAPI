using BookieAPI.Constants;
using BookieAPI.Controllers.Utils.ModelUtils;
using BookieAPI.Filters.ErrorHandlers;
using BookieAPI.Models.Context;
using BookieAPI.Models.ResponseModels;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BookieAPI.Controllers
{
    public class BookUpdatePictureController : ApiController
    {
        Context context = new Context();
        PictureUploadResponse response = new PictureUploadResponse();

        [HttpGet]
        [HttpPost]
        public async Task<PictureUploadResponse> PostFile(string email, string password, string imageName, int bookID)
        {
            ErrorHandler.WithGet(context, email, password, imageName, bookID.ToString())
                        .isValuesNullOrEmpty()
                        .isEmailValid()
                        .isUserExist()
                        .isUserVerified()
                        .isUserLocationExist()
                        .addOnErrorListener(new EventHandler(OnError))
                        .Check();

            if(response.errorCode == ResponseConstant.ERROR_NONE)
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }
                string root = HttpContext.Current.Server.MapPath("~/Images/");
                var provider = new MultipartFormDataStreamProvider(root);
                try
                {
                    StringBuilder sb = new StringBuilder(); // Holds the response body

                    // Read the form data and return an async task.
                    await Request.Content.ReadAsMultipartAsync(provider);

                    // This illustrates how to get the form data.
                    foreach (var key in provider.FormData.AllKeys)
                    {
                        foreach (var val in provider.FormData.GetValues(key))
                        {
                            sb.Append(string.Format("{0}: {1}\n", key, val));
                        }
                    }
                    var file = provider.FileData.FirstOrDefault();
                    var fileName = file.LocalFileName;
                    string path = root + "BookPictures/";
                    string pictureName = imageName;
                    string thumbnailPath = root + "BookPicturesThumbnails/";
                    string thumbnailName = imageName;
                    File.Copy(file.LocalFileName, path + pictureName);

                    File.Move(path + pictureName, Path.ChangeExtension(path + pictureName, ".jpg"));
                    File.Move(file.LocalFileName, thumbnailPath + thumbnailName);
                    File.Move(thumbnailPath + thumbnailName, Path.ChangeExtension(thumbnailPath + thumbnailName, ".jpg"));
                    thumbnailName = Path.GetFileNameWithoutExtension(imageName);

                    ImageResize(thumbnailPath + thumbnailName + ".jpg");

                    if (File.Exists(thumbnailPath + thumbnailName + ".jpg"))
                    {
                        File.Delete(thumbnailPath + thumbnailName + ".jpg");
                    }
                    string bookPictureURL = BookUtils.GetBookPictureURL(context, bookID);
                    if (!string.IsNullOrEmpty(bookPictureURL))
                    {
                        if (File.Exists(path + bookPictureURL))
                            File.Delete(path + bookPictureURL);
                    }
                    string bookPictureThumbnailURL = BookUtils.GetBookPictureThumbnailURL(context, bookID);
                    if (!string.IsNullOrEmpty(bookPictureThumbnailURL))
                    {
                        if (File.Exists(thumbnailPath + bookPictureThumbnailURL))
                            File.Delete(thumbnailPath + bookPictureThumbnailURL);
                    }
                    pictureName = pictureName.Remove(pictureName.Length - 4);
                    string[] urls = BookUtils.UpdateBookPicture(context, pictureName + ".jpg", thumbnailName + "_thumbnail.jpg", bookID);
                    response.error = false;
                    response.pictureURL = urls[0];
                    response.thumbnailURL = urls[1];

                }
                catch (System.Exception ex)
                {
                    OnError(this, new Filters.ErrorHandlers.ErrorEventArgs(ResponseConstant.ERROR_INVALID_REQUEST));
                }
            }


            if (response.error)
            {
                return response;
            }
            else
            {
                return response;
            }
        }

        private void OnError(Object s, EventArgs e)
        {
            Filters.ErrorHandlers.ErrorEventArgs error = (Filters.ErrorHandlers.ErrorEventArgs)e;
            if (error.errorCode != ResponseConstant.ERROR_NONE)
            {
                response.errorCode = error.errorCode;
            }
            else
            {
                response.errorCode = ResponseConstant.ERROR_UNKNOWN;
            }
        }

        public void ImageResize(string path)
        {
            string a = path;
            if (Path.GetExtension(path) != null)
            {
                a = a.Remove(a.LastIndexOf('.'));
            }
            try
            {
                Image image = Image.FromFile(path);
                using (Image newImage = new Bitmap(image, 360, 480))
                {
                    image.Dispose();
                    newImage.Save(a + "_thumbnail.jpg", ImageFormat.Jpeg);
                    newImage.Dispose();
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
