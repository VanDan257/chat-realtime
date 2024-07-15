import { Call } from "./call";

export interface GroupCall {
    Code: string;
    Type: string;
    Avatar: string;
    Name: string;
    Created: any;
    CreatedBy: string;
    LastActive: any;

    Calls: Call[];
    LastCall: Call;
}
