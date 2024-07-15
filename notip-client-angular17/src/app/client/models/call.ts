import { User } from "./user";

export interface Call {
    Id: number;
    GroupCallCode: string;
    UserCode: string;
    Url: string;
    Status: string;
    Created: any;

    User: User;
}