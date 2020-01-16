import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';
import { FormTypeSignees, SoilReportBundle, SoilReport, TestResults } from './soildata.report';

@Injectable({
  providedIn: 'root'
})
export class SoildataService {

  private baseUrl = '/api/Soildata/';

  private handleError: HandleError;

  constructor( 
      private http: HttpClient, 
      private location:Location,
      httpErrorHandler: HttpErrorHandler
      ) {
          this.handleError = httpErrorHandler.createHandleError('SoildataService');
      }
 
      addresses(planningUnitId:number = 0):Observable<FarmerAddress[]>{
        var url = this.baseUrl + "addresses/"+planningUnitId;
        return this.http.get<FarmerAddress[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('addresses', []))
            );
      }
      
      addaddress( address:FarmerAddress ):Observable<FarmerAddress>{
        return this.http.post<FarmerAddress>(this.location.prepareExternalUrl(this.baseUrl + 'addaddress/'), address)
            .pipe(
                catchError(this.handleError('add', <FarmerAddress>{}))
            );
      }

      updateaddress(id:number, address:FarmerAddress):Observable<FarmerAddress>{
        var url = this.baseUrl + 'updateaddress/' + id;
        return this.http.put<FarmerAddress>(this.location.prepareExternalUrl(url), address)
                .pipe(
                    catchError(this.handleError('update', address))
                );
      }
      
      addNote( note:CountyNote ):Observable<CountyNote>{
        return this.http.post<CountyNote>(this.location.prepareExternalUrl(this.baseUrl + 'addNote/'), note)
            .pipe(
                catchError(this.handleError('addNote', <CountyNote>{}))
            );
      }

      updateNote(id:number, note:CountyNote):Observable<CountyNote>{
        var url = this.baseUrl + 'updateNote/' + id;
        return this.http.put<CountyNote>(this.location.prepareExternalUrl(url), note)
                .pipe(
                    catchError(this.handleError('update', note))
                );
      }

      updateCropNote(id:number, note:SoilReport):Observable<SoilReport>{
        var url = this.baseUrl + 'updatecropnote/' + id;
        return this.http.put<SoilReport>(this.location.prepareExternalUrl(url), note)
                .pipe(
                    catchError(this.handleError('update', <SoilReport>{}))
                );
      }

      updateBundleFarmer(id:number, address:FarmerAddress):Observable<SoilReportBundle>{
        var url = this.baseUrl + 'updatebundleaddress/' + id;
        return this.http.put<SoilReportBundle>(this.location.prepareExternalUrl(url), address)
                .pipe(
                    catchError(this.handleError('update', <SoilReportBundle>{}))
                );
      }



      deleteNote(id:number):Observable<{}>{
        var url = this.baseUrl + "deleteNote/" + id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('delete note'))
            );
      }
      
      notesByCounty(id:number = 0):Observable<CountyNote[]>{
        var url = this.baseUrl + "notesByCounty/"+id;
        return this.http.get<CountyNote[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('notes by county', []))
            );
      }

      updateSignees(signees:Object, countId:number = 0):Observable<FormTypeSignees[]>{
        var url = this.baseUrl + 'updateSignees/' + countId;
        return this.http.post<FormTypeSignees[]>(this.location.prepareExternalUrl(url), signees)
                .pipe(
                    catchError(this.handleError('update signees', []))
                );
      }

      signeesByCounty(id:number = 0):Observable<FormTypeSignees[]>{
        var url = this.baseUrl + "signeesByCounty/"+id;
        return this.http.get<FormTypeSignees[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('Form Type Signees by County', []))
            );
      }
      labResults(reportId:number):Observable<TestResults[]>{
        var url = this.baseUrl + "labResults/"+reportId;
        return this.http.get<TestResults[]>(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('Lab Data Results', []))
            );
      }

      getCustom(searchParams?:{}):Observable<SoilReportBundle[]>{
        var url = this.baseUrl + "GetCustom/";
        return this.http.get<SoilReportBundle[]>(this.location.prepareExternalUrl(url), this.addParams(searchParams))
            .pipe(
                catchError(this.handleError('getCustom', []))
            );
      }
      pdf(id:string):Observable<Blob>{
        return this.http.get(this.location.prepareExternalUrl('/api/PdfSoilData/report/' + id ), {responseType: 'blob'})
            .pipe(
                catchError(this.handleError('pdf', <Blob>{}))
            );
    }

      private addParams(params:{}){
        let searchParams = {};
        for(let p in params){
            searchParams[p] = params[p];
        }
        return  {params: searchParams};
      }

}

export interface CountyCode{
    id:number;
    code:string;
    name:string;
    countyID:number;
    planningUnitId:number;
}

export class FarmerAddress{
    id:number;
    countyCode:CountyCode;
    first:string;
    mi:string;
    last:string;
    title:string;
    modifier:string;
    company:string;
    address:string;
    city:string;
    st:string;
    status:string;
    workNumber:string;
    duplicateHouseHold:string;
    homeNumber:string;
    fax:string;
    farmerID:string;
    zip:string;
    emailAddress:string;
    latitude:string;
    longitude:string;
    altitude:string;
    farmerData:string;
}

export interface CountyNote{
    id:number;
    countyCode:CountyCode;
    name:string;
    note:string;
}








