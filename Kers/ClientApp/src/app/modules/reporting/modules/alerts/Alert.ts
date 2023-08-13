import { KersUser } from "../admin/users/users.service";

export class Alert{
    id:number;
    message:string;
    urlRoute:string;
    alertType:number;
    start:Date;
    end:Date;
    moreInfoUrl:string;
    active:boolean;
    createdBy:KersUser;
    created:Date;
    lastUpdated:Date;
    employeePositionId?:number;
    zEmpRoleTypeId?:number;
    isCountyStarr?:number;
}

export class AlertRoute{
    id:number;
    name:string;
    urlRoute:string;
    active:boolean;
}

export const AlertTypes:AlertType[] =   [
    {
        id:1,name:"Note (green)",class:"alert-success"
    },
    {
        id:2,name:"Warning (blue)",class:"alert-info"
    },
    {
        id:3,name:"Alert (orange)",class:"alert-warning"
    },
    {
        id:4,name:"Urgent (red)",class:"alert-danger"
    }
                                        ];


export class AlertType{
    id:number;
    name: string;
    class: string;
}