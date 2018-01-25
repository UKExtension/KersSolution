import {Injectable} from '@angular/core';
import {Http, Headers, URLSearchParams} from '@angular/http';

@Injectable ()
export class AuthHttp{
    http = null;
    authKey = "auth";
    constructor (http: Http){
        this.http = http;
    }

    get(url, opts={}){
        this.configureAuth(opts, "get");
        return this.http.get(url, opts);
    }

    post(url, data, opts = {}){
        this.configureAuth(opts);
        return this.http.post(url, data, opts);
    }

    put(url, data, opts = {}){
        this.configureAuth(opts);
        return this.http.put(url, data, opts);
    }

    delete(url, opts = {}){
        this.configureAuth(opts);
        return this.http.delete(url, opts);
    }

    getBy(url, params:{}){
        let searchParams = new URLSearchParams();
        for(let p in params){
            searchParams.append(p, params[p]);
        }
        return this.get(url, {search: searchParams})
    }


    configureAuth(opts:any, request="post"){
        if (typeof window !== 'undefined') {
            var i = localStorage.getItem(this.authKey);
            if( i != null ){
                var auth = JSON.parse(i);
                if( auth.access_token != null) {
                    if(opts.headers == null){
                        opts.headers = new Headers();
                    }
                }
                opts.headers.set("Authorization", `Bearer ${auth.access_token}`);
            }
        }
        // Prevent IE from caching requests
        if(request == "get"){
            opts.search = opts.search || new URLSearchParams();
            opts.search.set('timestamp', (new Date()).getTime());
        }
    }
}