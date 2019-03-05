import {ExtensionEvent} from '../events/extension-event';
import { User, PlanningUnit } from '../user/user.service';

export class Training extends ExtensionEvent{
    classicInServiceTrainingId:number;
    submittedBy: User;
    approvedBy:User;
    approvedDate?:Date;
    tID:string;
    tStatus:string;
    sessionCancelledDate?:Date;
    trainDateBegin:string;
    trainDateEnd:string;
    registerCutoffDaysId?:number;
    registerCutoffDays:TainingRegisterWindow;
    cancelCutoffDaysId?:number;
    cancelCutoffDays: TrainingCancelEnrollmentWindow;
    iHourId?:number;
    iHour:TainingInstructionalHour;
    seatLimit?:number;
    tLocation:string;
    tTime:string;
    day1:string;
    day2:string;
    day3:string;
    day4:string;
    tContact:string;
    tAudience:string;
    enrollment:TrainingEnrollment[];
    qualtricsSurveyID:string;
    evaluationLink:string;
}

export class TainingRegisterWindow{
    id:number;
    registerDaysVal:string;
    registerDaysTxt:string;

}

export class TrainingCancelEnrollmentWindow{
    id:number;
    cancelDaysVal: number;
    cancelDaysTxt:string;
}

export class TainingInstructionalHour{
    id:number;
    iHoursTxt:string;
    iHourValue:number;
}

export class TrainingEnrollment{
    id:number;
    rDT?:Date;
    puid:string;
    PlanningUnitId:number;
    planningUnit:PlanningUnit;
    attendie:User;
    trainingId:string;
    eStatus:string;
    enrolledDate:Date;
    cancelledDate:Date;
    attended?:boolean;
    evaluationMessageSent?:boolean;
}