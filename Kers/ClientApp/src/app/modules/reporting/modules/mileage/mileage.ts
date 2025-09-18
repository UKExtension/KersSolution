import { ProgramCategory } from '../admin/programs/programs.service';
import { ExtensionEventLocation } from '../events/extension-event';
import { ExpenseFundingSource } from '../expense/expense.service';
import { User } from '../user/user.service';

export interface Mileage{
    id:number,
    expenseDate:Date,
    expenseId:number,
    startingLocationType:number,
    expenseLocation:string,
    programCategoryId: number,
    businessPurpose: string,
    fundingSourceNonMileageId:number,
    fundingSourceMileageId:number
    mileage:number,
    registration:number,
    lodging:number,
    otherExpenseCost:number,
    otherExpenseExplanation:string,
    departTime?:Date,
    returnTime?:Date,
    isOvernight: boolean,
    comment: string,
    vehicleType?:number,
    countyVehicleId?:number,
    startingLocation:ExtensionEventLocation,
    startingLocationId:number,
    segments:MileageSegment[]


}

export interface MileageBundle{
    id:number,
    kersUserId:number,
    kersUser:User,
    expenseDate:Date,
    vehicleType:number,
    revisions:Mileage[],
    lastRevisionId:number,
    lastRevision:Mileage,
    created:Date,
    updated:Date
}
export interface MileageSegment{
    id:number,
    locationId:number,
    location:ExtensionEventLocation,
    programCategoryId:number
    programCategory: ProgramCategory,
    businessPurpose:string,
    fundingSourceId:number,
    fundingSource: ExpenseFundingSource
    mileage:number,
    order:number
}

export interface MileageMonth{
    month:number;
    year:number;
    date:Date;
    expenses:Mileage[];
}