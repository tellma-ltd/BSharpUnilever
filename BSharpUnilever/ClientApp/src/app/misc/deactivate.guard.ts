import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, CanDeactivate } from '@angular/router';
import { Observable } from 'rxjs';
import { DataService } from '../data/data.service';

@Injectable({
  providedIn: 'root'
})
export class DeactivateGuard implements CanDeactivate<ICanDeactivate> {

  constructor(private data: DataService) { }

  canDeactivate(component: ICanDeactivate) {

    if (this.data.isSaving) {
      return false;
    } else {
      return component.canDeactivate ? component.canDeactivate() : true;
    }
  }
}

export interface ICanDeactivate {
  canDeactivate: () => Observable<boolean> | Promise<boolean> | boolean;
}
