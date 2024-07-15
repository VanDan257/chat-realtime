import { Message } from "./message";
import { User } from "./user";

export interface Group {
    Code: string;
    Type: string;
    Avatar: string;
    Name: string;
    Created: Date;
    CreatedBy: string;
    LastActive: Date;

    Users: User[];
    LastMessage: Message;
}
