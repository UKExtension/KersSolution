import { User, zEmpRoleType, Image } from '../user/user.service';

export class LadderApplication{
    id:number;
    kersUser:User;
    kersUserId:number;
    positionNumber:string;
    lastPromotion:Date;
    ladderLevel:LadderLevel;
    ladderLevelId:number;
    lastStage:LadderStage;
    lastStageId:number;
    stages:LadderApplicationStage[];
    ratings:LadderPerformanceRating[];
    images:LadderImage[];
}

export class LadderImage{
    id:number;
    uploadImage:Image;
    created:Date;
}
export class LadderPerformanceRating{
    id:number;
    year:string;
    ratting:string;
    order:number;
}

export class LadderLevel{
    id:number;
    name:string;
    order:number;
}

export class LadderApplicationStage{
    id:number;
    ladderApplication:LadderApplication;
    created:Date;
    ladderStage:LadderStage;
    kersUser:User;
    note:string;

}

export class LadderStage{
    id:number;
    name:string;
    order:number;
    LadderStageRoles:LadderStageRole[];

}

export class LadderStageRole{
    id:number;
    ladderStage:LadderStage;
    ladderStageId:number;
    zEmpRoleType:zEmpRoleType;
    zEmpRoleTypeId:number;

}