import { User } from './user';

export interface Message {
    Id: number;
    Type: string;
    GroupCode: string;
    Content: string;
    Path: string;
    Created: Date;
    CreatedBy: String;
    IsNew: boolean;

    SendTo: string;
    UserCreatedBy: User;
}