import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { Http, Response, Headers, RequestOptions, URLSearchParams, ResponseContentType } from '@angular/http';
import {Observable} from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/of';
import {AuthHttp} from '../../../authentication/auth.http';
import {MajorProgram, ProgramCategory } from '../admin/programs/programs.service';


@Injectable()
export class ExpenseService {

    private baseUrl = '/api/expense/';


    constructor( 
        private http:AuthHttp, 
        private location:Location
        ){}


    byRevId(id:number):Observable<Expense>{
        var url = this.baseUrl + 'byrevid/' + id;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Expense>res.json() )
                .catch(this.handleError);
    }

    add( expense:Expense ){
        return this.http.post(this.location.prepareExternalUrl(this.baseUrl), JSON.stringify(expense), this.getRequestOptions())
                    .map( res => <Expense>res.json() )
                    .catch(this.handleError);
    }

    update(id:number, expense:Expense){
        var url = this.baseUrl + id;
        return this.http.put(this.location.prepareExternalUrl(url), JSON.stringify(expense), this.getRequestOptions())
                    .map( res => {
                        return <Expense> res.json();
                    })
                    .catch(this.handleError);
    }

    delete(id:number){
        var url = this.baseUrl + id;
        return this.http.delete(this.location.prepareExternalUrl(url), this.getRequestOptions())
                    .map( res => {
                        return res;
                    })
                    .catch(this.handleError);
    }

    latest(skip:number = 0, take:number = 6){
        var url = this.baseUrl + 'latest/' + skip + '/' + take;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Expense[]>res.json() )
                .catch(this.handleError);
    }
    num(){
        var url = this.baseUrl + 'numb';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <number>res.json() )
                .catch(this.handleError);
    }

    fundingSources():Observable<ExpenseFundingSource[]>{
        var url = this.baseUrl + 'FundingSource';
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <ExpenseFundingSource[]>res.json())
                .catch(this.handleError);
    }
    mealRates(userid:number = 0):Observable<ExpenseMealRate>{
        var url = this.baseUrl + 'MealRate' + '/' + userid;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <ExpenseMealRate>res.json())
                .catch(this.handleError);
    }

    mileageRate(month:number, year:number, userid:number = 0){
        var url = this.baseUrl + 'mileagerate' + '/' + month + '/' + year + '/' + userid;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <number>res.json())
                .catch(this.handleError);
    }

    snapHours(month:number, userid:number = 0){
        var url = '/api/SnapClassic/hours/' + month + '/' + userid;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => res.json())
                .catch(this.handleError);
    }

    pdf(year:number, month:number, id:number = 0){
        return this.http.get(this.location.prepareExternalUrl('/api/pdf/expenses/' + year + '/' + month + '/' + id ), { responseType: ResponseContentType.Blob })
                .map((res:Response) => {
                    var pd = res.blob();
                    return pd;
                })
                .catch(this.handleError);
    }

    pdfTrip(year:number, month:number, id:number = 0, isOvernight:boolean = false){
        return this.http.get(this.location.prepareExternalUrl('/api/PdfTripExpenses/tripexpenses/' + year + '/' + month + '/' + id + '/' + isOvernight ), { responseType: ResponseContentType.Blob })
                .map((res:Response) => {
                    var pd = res.blob();
                    return pd;
                })
                .catch(this.handleError);
    }

    yearsWithExpenses(id:number = 0){
        var url = this.baseUrl + 'years/' + id;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => res.json())
                .catch(this.handleError);
    }
    monthsWithExpenses(year, userid:number = 0){
        var url = this.baseUrl + 'months/' + year + '/' + userid;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => res.json())
                .catch(this.handleError);
    }

    expensesPerMonth(month:number, year:number = 2017, userid:number = 0, orderBy:string = 'desc') : Observable<Expense[]>{
        var url = this.baseUrl + 'permonth/' + year + '/' + month + '/' + userid + '/' + orderBy;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => <Expense[]>res.json() )
                .catch(this.handleError);
    }


    // Expense Summaries Per Fiscal Year
    // Empty String for the Fiscal Year Name defaults to Current Fiscal Year
    fiscalYearSummaries(userId:number = 0, fiscalYearName:string = ""){
        var url = this.baseUrl + 'fysummaries/' + userId + '/' + fiscalYearName;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => {
                    var ret = <ExpenseSummary[]>res.json();
                    return ret;
                } )
                .catch(this.handleError);
    }


    SummariesPerPeriod(start:Date, end:Date, userId:number = 0){
        var url = this.baseUrl + 'summariesPerPeriod/' + start.toISOString() + '/' + end.toISOString() + '/' + userId;
        return this.http.get(this.location.prepareExternalUrl(url))
                .map(res => {
                    var ret = <ExpenseSummary[]>res.json();
                    return ret;
                } )
                .catch(this.handleError);
    }


    getRequestOptions(){
        return new RequestOptions(
            {
                headers: new Headers({
                    "Content-Type": "application/json; charset=utf-8"
                })
            }
        )
    }

    handleError(err:Response){
        console.error(err);
        return Observable.throw(err.json().error || 'Server error');
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
    returnTime?:Date
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