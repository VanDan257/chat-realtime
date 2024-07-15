import { Message } from './../../client/models/message';
import { User } from "../../client/models/user";

export interface UserAdmin{
    User: User;
    MessageCount: number;
}