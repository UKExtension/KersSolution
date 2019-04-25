import { User } from '../../user/user.service';

export class MessageTemplate{
    id:number;
    code:string;
    subject:string;
    bodyHtml:string;
    bodyText:string;
    createdBy:User;
    updatedBy:User;
    created:Date;
    updated:Date;
}