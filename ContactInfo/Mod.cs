using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactInfo
{
    public static class Mod
    {
        public static int checkNullInt(object p_field)
        {
            int result = 0;
            if ((p_field == null) || (p_field == DBNull.Value))
            {
                result = 0;
            }
            else
            {
                if (int.TryParse(p_field.ToString(), out result))
                {
                }
                else
                {
                    result = 0;
                }
            }

            return result;
        }

        public static String checkNullString(object p_field)
        {
            String result = "";
            if ((p_field == null) || (p_field == DBNull.Value))
            {
                result = "";
            }
            else
            {
                result = p_field.ToString();
            }

            return result;
        }

        public static int MakeInt(object p_field)
        {
            int result = -1;
            if ((p_field == null) || (p_field == DBNull.Value))
            {
                result = -1;
            }
            else
            {
                if (int.TryParse(p_field.ToString(), out result))
                {
                }
                else
                {
                    result = -1;
                }
            }

            return result;
        }

        public static String CheckApos(object p_field)
        {
            String result = "";
            if ((p_field == null) || (p_field == DBNull.Value))
            {
                result = "";
            }
            else
            {
                result = p_field.ToString().Replace("'","''");
            }

            return result;
        }

        public static string ImageToBase64(Image image) //, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                //image.Save(ms, format);
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

    }
}
