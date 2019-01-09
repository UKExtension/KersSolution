import { Injectable} from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../../core/services/http-error-handler.service';
import { NavigationService, NavSection, NavGroup, NavItem } from '../../../components/reporting-navigation/navigation.service';

@Injectable()
export class AdminNavigationService {

    private baseUrl = '/api/nav/';
    private handleError: HandleError;

    private navSections:NavSection[];

    constructor( 
        private http: HttpClient, 
        private location:Location,
        httpErrorHandler: HttpErrorHandler
        ) {
            this.handleError = httpErrorHandler.createHandleError('AdminNavigationService');
        }

    nav():Observable<NavSection[]>{
        var url = this.baseUrl + 'all/';
        return this.http.get<NavSection[]>(this.location.prepareExternalUrl(url))
        .pipe(
            tap(res => {
                var navSections = res;
                this.navSections = navSections;
            }),
            catchError(this.handleError('nav', []))
        );
    }

    // Navigation SECTION crud
    addSection(section:NavSection):Observable<NavSection>{
        var url = this.baseUrl + 'section/';
        return this.http.post<NavSection>(this.location.prepareExternalUrl(url), section)
            .pipe(
                tap(res => {
                    this.navSections.push(res);
                }),
                catchError(this.handleError('addSection', section))
            );
                   
                    
    }
    updateSection(id:number, section:NavSection):Observable<NavSection>{
        var url = this.baseUrl + 'section/' + id;
        return this.http.put<NavSection>(this.location.prepareExternalUrl(url), section)
            .pipe(
                tap(res => {
                    this.navSections = null;
                }),
                catchError(this.handleError('updateSection', section))
            );            

    }

    deleteSection(section:NavSection):Observable<{}>{
        var url = this.baseUrl + 'section/' + section.id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('deleteSection'))
            );
    }

    // Navigation GROUP crud

    addGroup(group:NavGroup, section:NavSection):Observable<NavGroup>{
        var url = this.baseUrl + 'group/'+section.id;
        return this.http.post<NavGroup>(this.location.prepareExternalUrl(url), group)
            .pipe(
                catchError(this.handleError('addGroup', group))
            );
                    
    }
    updateGroup(id:number, group:NavGroup):Observable<NavGroup>{
        var url = this.baseUrl + 'group/' + id;
        return this.http.put<NavGroup>(this.location.prepareExternalUrl(url), group)
            .pipe(
                catchError(this.handleError('updateGroup', group))
            );
    }

    deleteGroup(group:NavGroup):Observable<{}>{
        var url = this.baseUrl + 'group/' + group.id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('deleteGroup'))
            );
    }


    // Navigation ITEM crud

    addItem(item:NavItem, group:NavGroup):Observable<NavItem>{
        var url = this.baseUrl + 'item/'+group.id;
        return this.http.post<NavItem>(this.location.prepareExternalUrl(url), item)
            .pipe(
                catchError(this.handleError('addItem', item))
            );
                    
    }
    updateItem(id:number, item:NavItem):Observable<NavItem>{
        var url = this.baseUrl + 'item/' + id;
        return this.http.put<NavItem>(this.location.prepareExternalUrl(url), item)
            .pipe(
                catchError(this.handleError('updateItem', item))
            );
    }

    deleteItem(item:NavItem):Observable<{}>{
        var url = this.baseUrl + 'item/' + item.id;
        return this.http.delete(this.location.prepareExternalUrl(url))
            .pipe(
                catchError(this.handleError('deleteGroup'))
            );
    }
}