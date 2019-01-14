import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';


@Injectable()
export class ExpenseService {

    private baseUrl = '/api/expense/';
    private handleError: HandleError;


    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('ExpenseService');
        }


    byRevId(id:number):Observable<Expense>{
        var url = this.baseUrl + 'byrevid/' + id;
        return this.http.get<Expense>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('byRevId', <Expense>{}))
            );
    }

    add( expense:Expense ):Observable<Expense>{
        return this.http.post<Expense>(this.location.prepareExternalUrl(this.baseUrl), expense)
            .pipe(
                catchError(this.handleError('add', <Expense>{}))
            );
    }

    update(id:number, expense:Expense):Observable<Expense>{
        var url = this.baseUrl + id;
        return this.http.put<Expense>(this.location.prepareExternalUrl(url), expense)
            .pipe(
                catchError(this.handleError('update', <Expense>{}))
            );
    }

    delete(id:number):Observable<{}>{
        var url = this.baseUrl + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('delete'))
            );
    }

    latest(skip:number = 0, take:number = 6):Observable<Expense[]>{
        var url = this.baseUrl + 'latest/' + skip + '/' + take;
        return this.http.get<Expense[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('latest', []))
            );
    }
    num():Observable<number>{
        var url = this.baseUrl + 'numb';
        return this.http.get<number>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('num', 0))
            );
    }

    fundingSources():Observable<ExpenseFundingSource[]>{
        var url = this.baseUrl + 'FundingSource';
        return this.http.get<ExpenseFundingSource[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('fundingSources', []))
            );
    }
    mealRates(userid:number = 0):Observable<ExpenseMealRate[]>{
        var url = this.baseUrl + 'MealRate' + '/' + userid;
        return this.http.get<ExpenseMealRate[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('mealRates', []))
            );
    }

    mileageRate(month:number, year:number, userid:number = 0):Observable<number>{
        var url = this.baseUrl + 'mileagerate' + '/' + month + '/' + year + '/' + userid;
        return this.http.get<number>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('mileageRate', 0))
            );
    }

    snapHours(month:number, userid:number = 0):Observable<number>{
        var url = '/api/SnapClassic/hours/' + month + '/' + userid;
        return this.http.get<number>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('snapHours', 0))
            );
    }

    pdf(year:number, month:number, id:number = 0):Observable<Blob>{
        return this.http.get(this.location.prepareExternalUrl('/api/pdf/expenses/' + year + '/' + month + '/' + id ), {responseType: 'blob'})
            .pipe(
                catchError(this.handleError('pdf', <Blob>{}))
            );
    }

    pdfTrip(year:number, month:number, id:number = 0, isOvernight:boolean = false):Observable<Blob>{
        return this.http.get(this.location.prepareExternalUrl('/api/PdfTripExpenses/tripexpenses/' + year + '/' + month + '/' + id + '/' + isOvernight ), {responseType: 'blob'})
            .pipe(
                catchError(this.handleError('pdfTrip', <Blob>{}))
            );
    }

    yearsWithExpenses(id:number = 0):Observable<string[]>{
        var url = this.baseUrl + 'years/' + id;
        return this.http.get<string[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('yearsWithExpenses', []))
            );
    }
    monthsWithExpenses(year, userid:number = 0):Observable<string[]>{
        var url = this.baseUrl + 'months/' + year + '/' + userid;
        return this.http.get<string[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('monthsWithExpenses', []))
            );
    }

    expensesPerMonth(month:number, year:number = 2017, userid:number = 0, orderBy:string = 'desc') : Observable<Expense[]>{
        var url = this.baseUrl + 'permonth/' + year + '/' + month + '/' + userid + '/' + orderBy;
        return this.http.get<Expense[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('expensesPerMonth', []))
            );
    }


    // Expense Summaries Per Fiscal Year
    // Empty String for the Fiscal Year Name defaults to Current Fiscal Year
    fiscalYearSummaries(userId:number = 0, fiscalYearName:string = ""):Observable<ExpenseSummary[]>{
        var url = this.baseUrl + 'fysummaries/' + userId + '/' + fiscalYearName;
        return this.http.get<ExpenseSummary[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('fiscalYearSummaries', []))
            );
    }


    SummariesPerPeriod(start:Date, end:Date, userId:number = 0):Observable<ExpenseSummary[]>{
        var url = this.baseUrl + 'summariesPerPeriod/' + start.toISOString() + '/' + end.toISOString() + '/' + userId;
        return this.http.get<ExpenseSummary[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('SummariesPerPeriod', []))
            );
    }


}

export interface Expense{
    id:number,
    expenseDate:Date,
    expenseId:number,
    startingLocationType:number,
    expenseLocation:string,
    isOvernight: boolean,
    programCategoryId: number,
    businessPurpose: string,
    comment: string,
    fundingSourceNonMileageId:number,
    fundingSourceMileageId:number
    mileage:number,
    registration:number,
    lodging:number,
    mealRateBreakfastId:number,
    mealRateBreakfast:ExpenseMealRate,
    mealRateBreakfastCustom:number,
    mealRateLunchId:number,
    mealRateLunch:ExpenseMealRate,
    mealRateLunchCustom:number,
    mealRateDinnerId:number,
    mealRateDinner:ExpenseMealRate,
    mealRateDinnerCustom:number,
    otherExpenseCost:number,
    otherExpenseExplanation:string,
    departTime?:Date,
    returnTime?:Date,
    vehicleType?:number,
    countyVehicleId?:number,
}

export interface ExpenseFundingSource{
    id:number,
    name:string,
    order:number
}

export interface ExpenseMealRate{
    id:number,
    description:string,
    breakfastRate:number,
    lunchRate:number,
    dinnerRate:number
}

export interface ExpenseMonth{
    month:number;
    year:number;
    date:Date;
    expenses:Expense[];
}

export interface ExpenseSummary{
    fundingSource: ExpenseFundingSource;
    miles: number;
    mileageCost: number;
    meals:number;
    lodging: number;
    registration:number;
    other: number;
    total: number;
}