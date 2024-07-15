export class Constants {
    static LOCAL_STORAGE_KEY = class {
      static SESSION = 'session';
      static TOKEN = 'token';
    };

    static TYPE_GROUP_CHAT = class {
      static SINGLE = 'single';
      static MULTI = 'multi';
    }

    static FRIEND_STATUS = class {
      static FRIENDREQUEST = "FRIEND_REQUEST";
      static BLOCKED = "BLOCKED";
      static FRIEND = "FRIEND";
    }

    static ROLE = class {
      static CLIENT = "CLIENT";
      static ADMIN = "ADMIN";
      static MODERATOR = "MODERATOR";
    }
}
