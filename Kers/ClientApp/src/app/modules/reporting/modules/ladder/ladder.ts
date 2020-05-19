import { User, zEmpRoleType, Image } from '../user/user.service';

export class LadderApplication{
    id:number;
    kersUser:User;
    kersUserId:number;
    positionNumber:string;
    programOfStudy:string;
    evidence:string;
    numberOfYears?:number;
    lastPromotion:Date;
    startDate:Date;
    draft:boolean;
    ladderLevel:LadderLevel;
    ladderLevelId:number;
    ladderEducationLevel:LadderEducationLevel;
    ladderEducationLevelId:number;
    lastStage:LadderStage;
    lastStageId:number;
    stages:LadderApplicationStage[];
    ratings:LadderPerformanceRating[];
    images:LadderImage[];
}

export class LadderEducationLevel{
    id:number;
    name:string;
    order:number;
}

export class LadderImage{
    id:number;
    uploadImage:UploadImage;
    uploadImageId:number;
    created:Date;
    description:string;
}
export class UploadImage{
    id:number;
    name:string;
}
export class LadderPerformanceRating{
    id:number;
    year:string;
    ratting:string;
    order?:number;
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
    ladderStageRoles:LadderStageRole[];

}

export class LadderStageRole{
    id:number;
    ladderStage:LadderStage;
    ladderStageId:number;
    zEmpRoleType:zEmpRoleType;
    zEmpRoleTypeId:number;

}