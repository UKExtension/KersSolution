import { ProgramCategory } from '../admin/programs/programs.service';
import { ExtensionEventLocation } from '../events/extension-event';
import { ExpenseFundingSource } from '../expense/expense.service';

export interface Mileage{
    id:number,
    expenseDate:Date,
    expenseId:number,
    isOvernight: boolean,
    comment: string,
    vehicleType?:number,
    countyVehicleId?:number,
    startingLocation:ExtensionEventLocation,
    segments:MileageSegment[]


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