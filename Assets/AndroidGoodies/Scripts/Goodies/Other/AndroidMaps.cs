#if UNITY_ANDROID
using UnityEngine;

namespace DeadMosquito.AndroidGoodies
{
    public static class AndroidMaps
    {
        public const int MinMapZoomLevel = 1;
        public const int MaxMapZoomLevel = 23;
        public const int DefaultMapZoomLevel = 7;

        private const string MapUriFormat = "geo:{0},{1}?z={2}";
        private const string MapUriFormatLabel = "geo:0,0?q={0},{1}({2})";
        private const string MapUriFormatAddress = "geo:0,0?q={0}";

        /// <summary>
        /// Checks if user has any maps apps installed.
        /// </summary>
        /// <returns><c>true</c>, if user has any maps apps installed., <c>false</c> otherwise.</returns>
        public static bool UserHasMapsApp()
        {
            if (AndroidUtils.IsNotAndroidCheck())
            {
                return false;
            }
            // Dummy intent just to check if any apps can handle the intent
            var intent = new AndroidIntent(AndroidIntent.ACTION_VIEW);
            var uri = AndroidUtils.UriParse(string.Format(MapUriFormat, 0, 0, DefaultMapZoomLevel));
            return intent.SetData(uri).ResolveActivity();
        }

        /// <summary>
        /// Show the map at the given longitude and latitude at a certain zoom level. 
        /// A zoom level of 1 shows the whole Earth, centered at the given lat,lng. The highest (closest) zoom level is 23.
        /// </summary>
        /// <param name="latitude">The latitude of the location. May range from -90.0 to 90.0.</param>
        /// <param name="longitude">The longitude of the location. May range from -180.0 to 180.0.</param>
        /// <param name="zoom">Zoom level.</param>
        public static void OpenMapLocation(float latitude, float longitude, int zoom = DefaultMapZoomLevel)
        {
            if (AndroidUtils.IsNotAndroidCheck())
            {
                return;
            }

            if (latitude < -90.0f || latitude > 90.0f)
            {
                throw new System.ArgumentException("Latitude must be from -90.0 to 90.0.");
            }
            if (longitude < -180.0f || longitude > 180.0f)
            {
                throw new System.ArgumentException("Longtitude must be from -180.0 to 180.0.");
            }
            if (zoom < MinMapZoomLevel || zoom > MaxMapZoomLevel)
            {
                throw new System.ArgumentException("Zoom level must be between 1 and 23");
            }

            var intent = new AndroidIntent(AndroidIntent.ACTION_VIEW);
            var uri = AndroidUtils.UriParse(string.Format(MapUriFormat, latitude, longitude, zoom));
            intent.SetData(uri);

            AndroidUtils.StartActivity(intent.JavaObj);
        }

        /// <summary>
        /// Show the map at the given longitude and latitude with a certain label.
        /// </summary>
        /// <param name="latitude">The latitude of the location. May range from -90.0 to 90.0.</param>
        /// <param name="longitude">The longitude of the location. May range from -180.0 to 180.0.</param>
        /// <param name="label">Label to mark the point.</param>
        public static void OpenMapLocationWithLabel(float latitude, float longitude, string label)
        {
            if (AndroidUtils.IsNotAndroidCheck())
            {
                return;
            }

            if (latitude < -90.0f || latitude > 90.0f)
            {
                throw new System.ArgumentException("Latitude must be from -90.0 to 90.0.");
            }
            if (longitude < -180.0f || longitude > 180.0f)
            {
                throw new System.ArgumentException("Longtitude must be from -180.0 to 180.0.");
            }
            if (string.IsNullOrEmpty(label))
            {
                throw new System.ArgumentException("Label must not be null or empty");
            }

            var intent = new AndroidIntent(AndroidIntent.ACTION_VIEW);
            var uri = AndroidUtils.UriParse(string.Format(MapUriFormatLabel, latitude, longitude, label));
            intent.SetData(uri);

            AndroidUtils.StartActivity(intent.JavaObj);
        }

        /// <summary>
        /// Opens the map location with the provided address.
        /// </summary>
        /// <param name="address">Address to open.</param>
        public static void OpenMapLocation(string address)
        {
            if (AndroidUtils.IsNotAndroidCheck())
            {
                return;
            }

            if (string.IsNullOrEmpty(address))
            {
                throw new System.ArgumentException("Address must not be null or empty");
            }

            // Note: All strings passed in the geo URI must be encoded. For example, the string 1st & Pike, Seattle should become 1st%20%26%20Pike%2C%20Seattle. 
            // Spaces in the string can be encoded with %20 or replaced with the plus sign (+).
            address = WWW.EscapeURL(address);

            var intent = new AndroidIntent(AndroidIntent.ACTION_VIEW);
            var uri = AndroidUtils.UriParse(string.Format(MapUriFormatAddress, address));
            intent.SetData(uri);

            AndroidUtils.StartActivity(intent.JavaObj);
        }
    }
}
#endif
