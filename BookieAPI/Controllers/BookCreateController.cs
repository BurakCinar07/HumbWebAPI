using BookieAPI.Constants;
using BookieAPI.Controllers.Utils;
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
    public class BookCreateController : ApiController
    {
        Context context = new Context();
        BookCreateResponse response = new BookCreateResponse();

        [HttpGet]
        [HttpPost]
        public async Task<BookCreateResponse> PostFile(string email, string password, string imageName, string bookName, string author, int bookState, int genreCode)
        {
            ErrorHandler.WithGet(context, email, password, imageName, bookName, author, bookState.ToString(), genreCode.ToString())
                        .isValuesNullOrEmpty()
                        .isEmailValid()
                        .isUserExist()
                        .isUserVerified()
                        .isUserLocationExist()
                        .addOnErrorListener(new EventHandler(OnError))
                        .Check();

            if (!TextUtils.SanitizeImageName(imageName).Equals(imageName))
            {
                OnError(this, new Filters.ErrorHandlers.ErrorEventArgs(InfiltratorConstant.ERROR_INJECTION));
            }
            else if (bookState != ResponseConstant.STATE_OPENED_TO_SHARE && bookState != ResponseConstant.STATE_CLOSED_TO_SHARE && bookState != ResponseConstant.STATE_READING)
            {
                OnError(this, new Filters.ErrorHandlers.ErrorEventArgs(InfiltratorConstant.ERROR_INJECTION));
            }
            else if (genreCode < 0 || genreCode > ResponseConstant.GENRE_COUNT)
            {
                OnError(this, new Filters.ErrorHandlers.ErrorEventArgs(InfiltratorConstant.ERROR_INJECTION));
            }
            else if (response.errorCode == ResponseConstant.ERROR_NONE)
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
                    pictureName = pictureName.Remove(pictureName.Length - 4);

                    int bookID = BookUtils.CreateBook(context, email, pictureName + ".jpg", thumbnailName + "_thumbnail.jpg", bookName, author, bookState, genreCode);

                    response.error = false;
                    response.book = BookUtils.GetBookModel(context, bookID);
                }
                catch (Exception ex)
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
