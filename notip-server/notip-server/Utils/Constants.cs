using Amazon.S3.Model;
using System;

namespace notip_server.Utils
{
    public class Constants
    {
        public class GroupType
        {
            public const string SINGLE = "single";
            public const string MULTI = "multi";
        }

        public class CallStatus
        {
            public const string IN_COMMING = "IN_COMMING";
            public const string OUT_GOING = "OUT_GOING";
            public const string MISSED = "MISSED";
        }
        
        public class FriendStatus
        {
            public const string FRIENDREQUEST = "FRIEND_REQUEST";
            public const string BLOCKED = "BLOCKED";
            public const string FRIEND = "FRIEND";
        }

        public class OrderBy
        {
            public const string ASC = "ASC";
            public const string DESC = "DESC";
        }

        public class Role
        {
            public const string CLIENT = "CLIENT";
            public const string ADMIN = "ADMIN";
            public const string MODERATOR = "MODERATOR";
        }

        public const string AVATAR_DEFAULT = "images/no_image.jpg";
        public const string AVATAR_GROUP = "images/groupuserimage.PNG";
    }
}
