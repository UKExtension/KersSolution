import {ExtensionEvent} from '../events/extension-event';
import { User, PlanningUnit } from '../user/user.service';

export class Training extends ExtensionEvent{
    classicInServiceTrainingId:number;
    submittedBy: User;
    submittedById:number;
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
    day1:string;
    day2:string;
    day3:string;
    day4:string;
    tContact:string;
    tAudience:string;
    enrollment:TrainingEnrollment[];
    qualtricsSurveyID:string;
    evaluationLink:string;
    createdDateTime:Date;
    sessions: TrainingSession[];
    trainingSession: TrainingSession[];
    trainingSessionWithTimes:TrainingSession[];
    surveyResults:TrainingSurveyResult[];
    etimezone:boolean;
}
export class TrainingSurveyResult{
    id:number;
    result:string;
    userId:number;
    user:User;
    training:Training;
    trainingId:number;
    created:Date;
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
    attendieId:number;
    trainingId:string;
    training: Training;
    eStatus:string;
    enrolledDate:Date;
    cancelledDate:Date;
    attended?:boolean;
    evaluationMessageSent?:boolean;
}

export class TrainingSearchCriteria{
    start: string;
    end: string;
    search: string = "";
    status: string;
    contacts: string = "";
    day?: number;
    order: string = 'dsc';
    withseats: boolean = false;
    attendance: boolean = false;
    admin: boolean = false;
  }

  export class TrainingSession{
      id:number;
      date: Date | Object;
      starttime:string;
      endtime:string;
      note:string;
      index:number;
      start:string;
      end:string;
  }