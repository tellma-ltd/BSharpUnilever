import { Component, OnDestroy, ViewChild } from '@angular/core';
import { of, Subject } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { DataService } from '../../data/data.service';
import { Product } from '../../data/entities/Product';
import { Store } from '../../data/entities/Store';
import {
  GeneratedDocument, SupportRequest, SupportRequestLineItem,
  supportRequestReasons, SupportRequestState
} from '../../data/entities/SupportRequest';
import { User } from '../../data/entities/User';
import { GlobalsResolverService } from '../../data/globals-resolver.service';
import { DetailsComponent } from '../../layouts/details/details.component';
import { SerialPipe } from '../../misc/serial.pipe';
import { cloneModel, downloadBlob } from '../../misc/util';

@Component({
  selector: 'b-support-request-details',
  templateUrl: './support-request-details.component.html',
  styles: []
})
export class SupportRequestDetailsComponent implements OnDestroy {

  @ViewChild(DetailsComponent)
  details: DetailsComponent;

  public PREFIX = 'SR';
  private notifyDestruct$ = new Subject<void>();
  public showSpinner = false;
  public userFormatter = (user: User) => `${user.FullName} (${user.Role})`;
  public storeFormatter = (store: Store) => store.Name;
  public productFormatter = (product: Product) => product.Description;

  constructor(private globals: GlobalsResolverService, private data: DataService) { }

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
  }

  enableEditButton = (model: SupportRequest) => {
    const currentRole = this.globals.currentUser.Role;
    if (model.State === SupportRequestState.Draft) {
      return true;
    } else if (model.State === SupportRequestState.Submitted) {
      return ['Manager', 'Administrator'].includes(currentRole);
    } else if (model.State === SupportRequestState.Approved) {
      return ['KAE', 'Administrator'].includes(currentRole);
    } else {
      return false;
    }
  }

  get reasons() {
    return Object.keys(supportRequestReasons).map(key =>
      ({ value: key, display: supportRequestReasons[key] }));
  }

  reasonDisplay(key: string): string {
    return !!key ? supportRequestReasons[key] : '';
  }

  onDownloadCreditNote(cn: GeneratedDocument) {
    this.showSpinner = true;
    this.data.supportrequests.getGeneratedDocument(cn.Id, this.notifyDestruct$).subscribe(
      (blob: Blob) => {
        this.showSpinner = false;
        const serial = new SerialPipe().transform(cn.SerialNumber, 'CN');
        const fileName = `${serial} ${new Date().toDateString()}.pdf`;
        downloadBlob(blob, fileName);
      },
      () => {
        this.showSpinner = false;
        this.details.showModalError('Could not download the file, please contact your IT department');
      }
    );
  }

  onAddLine(model: SupportRequest) {
    const newLine = new SupportRequestLineItem();
    newLine['isNew'] = true; // This focuses the new line

    if (!model.LineItems) {
      model.LineItems = [];
    }

    model.LineItems.push(newLine);
  }

  onDeleteLine(index: number, model: SupportRequest) {

    const lineItems = model.LineItems;
    lineItems.splice(index, 1);
  }

  isVisibleSubmit(model: SupportRequest): boolean {
    const currentRole = this.globals.currentUser.Role;
    return !!model.Id &&
      [SupportRequestState.Draft].includes(model.State) &&
      ['KAE', 'Administrator'].includes(currentRole);
  }

  onSubmit(model: SupportRequest) {

    this.goToState(model, SupportRequestState.Submitted);
  }

  isVisibleApprove(model: SupportRequest) {
    const currentRole = this.globals.currentUser.Role;
    return !!model.Id &&
      [SupportRequestState.Draft, SupportRequestState.Submitted].includes(model.State) &&
      ['Manager', 'Administrator'].includes(currentRole);
  }

  onApprove(model: SupportRequest) {
    this.goToState(model, SupportRequestState.Approved);
  }

  isVisibleReject(model: SupportRequest) {
    const currentRole = this.globals.currentUser.Role;

    return !!model.Id &&
      [SupportRequestState.Submitted].includes(model.State) &&
      ['Manager', 'Administrator'].includes(currentRole);
  }

  onReject(model: SupportRequest) {
    this.goToState(model, SupportRequestState.Rejected);
  }

  isVisiblePost(model: SupportRequest) {
    const currentRole = this.globals.currentUser.Role;

    return !!model.Id &&
      ([SupportRequestState.Approved].includes(model.State) ||
        ([SupportRequestState.Draft].includes(model.State) && model.Reason === 'FB')) &&
      ['KAE', 'Administrator'].includes(currentRole);
  }

  onPost(model: SupportRequest) {
    this.goToState(model, SupportRequestState.Posted);
  }

  isVisibleUnReject(model: SupportRequest) {
    const currentRole = this.globals.currentUser.Role;

    return !!model.Id &&
      [SupportRequestState.Rejected].includes(model.State) &&
      ['Manager', 'Administrator'].includes(currentRole);
  }

  onUnReject(model: SupportRequest) {
    this.goToState(model, SupportRequestState.Submitted);
  }

  isVisibleCancel(model: SupportRequest) {
    const currentRole = this.globals.currentUser.Role;

    return !!model.Id &&
      [SupportRequestState.Draft].includes(model.State) &&
      ['KAE', 'Manager', 'Administrator'].includes(currentRole);
  }

  onCancel(model: SupportRequest) {
    this.goToState(model, SupportRequestState.Canceled);
  }

  isVisibleUnCancel(model: SupportRequest) {
    const currentRole = this.globals.currentUser.Role;

    return !!model.Id &&
      [SupportRequestState.Canceled].includes(model.State) &&
      ['KAE', 'Manager', 'Administrator'].includes(currentRole);
  }

  onUnCancel(model: SupportRequest) {
    this.goToState(model, SupportRequestState.Draft);
  }

  isVisibleUnPost(model: SupportRequest) {
    const currentRole = this.globals.currentUser.Role;

    return !!model.Id &&
      [SupportRequestState.Posted].includes(model.State) &&
      ['KAE', 'Manager', 'Administrator'].includes(currentRole);
  }

  onUnPost(model: SupportRequest) {
    const confirmed = confirm('This action will invalidate the generated credit note, are you sure you want to proceed?');
    if (confirmed) {
      const stateChanges = model.StateChanges;
      const previousState = stateChanges[stateChanges.length - 1].FromState;
      this.goToState(model, previousState);
    }
  }

  isVisibleReturn(model: SupportRequest) {
    const currentRole = this.globals.currentUser.Role;
    const approved = SupportRequestState.Approved;
    const submitted = SupportRequestState.Submitted;


    return !!model.Id &&
      (model.State === approved && ['KAE', 'Administrator'].includes(currentRole)) ||
      (model.State === submitted && ['Manager', 'Administrator'].includes(currentRole));
  }

  onReturn(model: SupportRequest) {
    const previousState = SupportRequestState.Draft;

    this.goToState(model, previousState);
  }

  isDraft(model): boolean {
    return model.State === 'Draft';
  }

  isVisibleHeaderRequestedValue(model: SupportRequest) {
    return ['DC', 'PS'].includes(model.Reason);
  }

  isVisibleHeaderApprovedValue(model: SupportRequest) {
    const currentRole = this.globals.currentUser.Role;
    return ['DC', 'PS'].includes(model.Reason) && this.isVisibleApprovedValue(model);
  }

  isVisibleHeaderUsedValue(model: SupportRequest) {
    const currentRole = this.globals.currentUser.Role;
    return (model.Reason === 'FB' &&
      model.State === SupportRequestState.Draft &&
      ['Administrator', 'KAE'].includes(currentRole)) ||
      (['DC', 'PS'].includes(model.Reason) && this.isVisibleUsedValue(model));
  }

  isVisibleRequestedSupport(model: SupportRequest) {
    return true;
  }

  isVisibleRequestedValue(model: SupportRequest) {
    return true;
  }

  isVisibleApprovedSupport(model: SupportRequest) {
    return this.isVisibleApprovedValue(model);
  }

  isVisibleApprovedValue(model: SupportRequest) {
    const currentRole = this.globals.currentUser.Role;



    return !(currentRole === 'KAE' && model.State === SupportRequestState.Submitted) &&
      ((['Administrator', 'Manager'].includes(currentRole) && model.State === SupportRequestState.Draft) ||
        [SupportRequestState.Submitted, SupportRequestState.Approved,
          SupportRequestState.Rejected, SupportRequestState.Posted].includes(model.State));
  }

  isVisibleUsedSupport(model: SupportRequest) {
    return this.isVisibleUsedValue(model);
  }

  isVisibleUsedValue(model: SupportRequest) {
    const currentRole = this.globals.currentUser.Role;
    return (model.State === SupportRequestState.Approved && ['Administrator', 'KAE'].includes(currentRole)) ||
      model.State === SupportRequestState.Posted;
  }

  isVisibleTable(model: SupportRequest) {
    return model.Reason === 'PR';
  }

  onRequestedSupportChange(li: SupportRequestLineItem) {
    li.RequestedValue = li.RequestedSupport * li.Quantity;
  }

  onApprovedSupportChange(li: SupportRequestLineItem) {
    li.ApprovedValue = li.ApprovedSupport * li.Quantity;
  }

  onUsedSupportChange(li: SupportRequestLineItem) {
    li.UsedValue = li.UsedSupport * li.Quantity;
  }

  onQuantityChange(li: SupportRequestLineItem) {
    this.onRequestedSupportChange(li);
    this.onApprovedSupportChange(li);
    this.onUsedSupportChange(li);
  }

  onReasonChange(newVal: any, model: SupportRequest) {
    const oldVal = model.Reason;
    model.Reason = null;

    if (oldVal !== 'PR' && newVal === 'PR') {
      model.LineItems = [];
    } else if (oldVal === 'PR' && newVal !== 'PR') {
      model.LineItems = [];
      model.LineItems.push(new SupportRequestLineItem());
    }

    model.Reason = newVal;
  }

  isEditableAccountExecutive(model: SupportRequest) {
    const currentRole = this.globals.currentUser.Role;
    return ['Manager', 'Administrator'].includes(currentRole) && model.State === SupportRequestState.Draft;
  }

  isEditableManager(model: SupportRequest) {
    const currentRole = this.globals.currentUser.Role;
    return model.State === SupportRequestState.Draft || (currentRole === 'Administrator' && model.State !== SupportRequestState.Submitted);
  }

  isEditableHeaderRequestedValue(model: SupportRequest) {
    return this.isEditableRequestedSupport(model);
  }

  isEditableHeaderApprovedValue(model: SupportRequest) {
    return this.isEditableApprovedSupport(model);
  }

  isEditableHeaderUsedValue(model: SupportRequest) {
    return this.isEditableUsedSupport(model);
  }

  isEditableRequestedSupport(model: SupportRequest) {
    return model.State === SupportRequestState.Draft;
  }

  isEditableApprovedSupport(model: SupportRequest) {
    const currentRole = this.globals.currentUser.Role;
    return ['Manager', 'Administrator'].includes(currentRole) &&
      [SupportRequestState.Draft, SupportRequestState.Submitted].includes(model.State);
  }

  isEditableUsedSupport(model: SupportRequest) {
    const currentRole = this.globals.currentUser.Role;
    return ['KAE', 'Administrator'].includes(currentRole) &&
      [SupportRequestState.Draft, SupportRequestState.Submitted, SupportRequestState.Approved].includes(model.State);
  }

  private goToState(model: SupportRequest, state: SupportRequestState) {
    const clone = cloneModel(model);
    clone.State = state;
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
