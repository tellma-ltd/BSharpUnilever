import { Component, OnDestroy, ViewChild } from '@angular/core';
import { of, Subject } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { DataService } from '../../data/data.service';
import { Product } from '../../data/entities/Product';
import { Store } from '../../data/entities/Store';
import { SupportRequest, SupportRequestLineItem, SupportRequestReason, SupportRequestState } from '../../data/entities/SupportRequest';
import { User } from '../../data/entities/User';
import { GlobalsResolverService } from '../../data/globals-resolver.service';
import { DetailsComponent } from '../../layouts/details/details.component';
import { cloneModel } from '../../misc/util';

@Component({
  selector: 'b-support-request-details',
  templateUrl: './support-request-details.component.html',
  styles: []
})
export class SupportRequestDetailsComponent implements OnDestroy {

  PREFIX = 'SR';

  @ViewChild(DetailsComponent)
  details: DetailsComponent;

  constructor(private globals: GlobalsResolverService, private data: DataService) { }

  private notifyDestruct$ = new Subject<void>();

  ngOnDestroy() {
    this.notifyDestruct$.next();
  }

  canDeactivate(): boolean {
    return this.details.canDeactivate();
  }

  createNew = () => {
    const result = new SupportRequest();
    result.SerialNumber = 0;
    result.State = SupportRequestState.Draft;
    result.Date = new Date().toISOString();
    result.LineItems = [new SupportRequestLineItem()];

    if (this.globals.currentUser.Role === 'KAE') {
      result.AccountExecutive = this.globals.currentUser;
    }

    return result;
  };

  get reasons() {
    return Object.keys(SupportRequestReason).map(key =>
      ({ value: key, display: SupportRequestReason[key] }));
  }

  reasonDisplay(key: string): string {
    return !!key ? SupportRequestReason[key] : '';
  }

  userFormatter = (user: User) => user.FullName;
  storeFormatter = (store: Store) => store.Name;
  productFormatter = (product: Product) => product.Description;

  addLine(model: SupportRequest) {
    let newLine = new SupportRequestLineItem();
    newLine['isNew'] = true; // This focuses the new line

    if (!model.LineItems)
      model.LineItems = [];

    model.LineItems.push(newLine);
  }

  deleteLine(index: number, model: SupportRequest) {

    let lineItems = model.LineItems;
    lineItems.splice(index, 1);
  }


  isVisibleSubmit(model: SupportRequest): boolean {
    return !!model.Id && model.State === SupportRequestState.Draft && this.globals.currentUser.Role !== 'Manager';
  }

  onSubmit(model: SupportRequest) {
    let clone = cloneModel(model);
    clone.State = SupportRequestState.Submitted;
    this.data.supportrequests.post(clone, this.notifyDestruct$).pipe(
      map((result: any) => {
        this.details.viewModel = result;
      }),
      catchError(friendlyError => {
        this.details.showModalError(friendlyError);
        return of(null);
      })
    ).subscribe();
  }
}
