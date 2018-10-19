import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgbDropdownModule, NgbPopoverModule, NgbModalModule, NgbCollapseModule } from '@ng-bootstrap/ng-bootstrap';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

import { AppComponent } from './app.component';
import { PageNotFoundComponent } from './screens/page-not-found/page-not-found.component';
import { SignInComponent } from './screens/sign-in/sign-in.component';
import { ForgotPasswordComponent } from './screens/forgot-password/forgot-password.component';
import { ConfirmEmailComponent } from './screens/confirm-email/confirm-email.component';
import { ResetPasswordComponent } from './screens/reset-password/reset-password.component';
import { PrivacyPolicyComponent } from './screens/privacy-policy/privacy-policy.component';
import { TermsOfServiceComponent } from './screens/terms-of-service/terms-of-service.component';
import { ShellComponent } from './screens/shell/shell.component';
import { UsersComponent } from './screens/users/users.component';
import { StoresComponent } from './screens/stores/stores.component';
import { ProductsComponent } from './screens/products/products.component';
import { SupportRequestsComponent } from './screens/support-requests/support-requests.component';
import { UserDetailsComponent } from './screens/user-details/user-details.component';
import { StoreDetailsComponent } from './screens/store-details/store-details.component';
import { ProductDetailsComponent } from './screens/product-details/product-details.component';
import { SupportRequestDetailsComponent } from './screens/support-request-details/support-request-details.component';
import { DataComponent } from './screens/data/data.component';
import { AuthComponent } from './layouts/auth/auth.component';
import { MasterComponent } from './layouts/master/master.component';
import { DetailsComponent } from './layouts/details/details.component';
import { BrandComponent } from './layouts/brand/brand.component';
import { SpinnerComponent } from './layouts/spinner/spinner.component';
import { FormFieldComponent } from './layouts/form-field/form-field.component';
import { DecimalEditorComponent } from './controls/decimal-editor/decimal-editor.component';
import { DetailsPickerComponent } from './controls/details-picker/details-picker.component';
import { Routes, RouterModule } from '@angular/router';
import { AuthNoGuard } from './misc/auth-no.guard';
import { AuthGuard } from './misc/auth.guard';
import { HttpRequestInterceptor } from './data/http-interceptor';
import { ErrorMessageComponent } from './layouts/error-message/error-message.component';
import { SuccessMessageComponent } from './layouts/success-message/success-message.component';
import { library } from '@fortawesome/fontawesome-svg-core';
import { ResetPasswordControlComponent } from './screens/reset-password/reset-password-control.component';
import {
  faExclamationTriangle,
  faSpinner,
  faSignInAlt,
  faSignOutAlt,
  faKey,
  faCheck,
  faPlus,
  faSyncAlt,
  faAngleDoubleLeft,
  faAngleLeft,
  faAngleRight,  faThLarge,
  faList,
  faEdit,
  faTrashAlt,
  faSave,
  faTimes,
  faDownload,
  faArrowCircleRight,
  faThumbsUp,
  faThumbsDown,
  faUndo,
  faClipboardCheck
} from '@fortawesome/free-solid-svg-icons';
import { GlobalsResolverService } from './data/globals-resolver.service';
import { SerialPipe } from './misc/serial.pipe';
import { DeactivateGuard } from './misc/deactivate.guard';
import { MyAccountComponent } from './screens/my-account/my-account.component';

// Icons to be used in the app
library.add(
  faExclamationTriangle, faSpinner, faSignInAlt, faSignOutAlt,
  faKey, faCheck, faPlus, faSyncAlt, faAngleDoubleLeft,
  faAngleLeft, faAngleRight, faThLarge, faList, faEdit, faTrashAlt,
  faSave, faTimes, faDownload, faArrowCircleRight, faThumbsUp, faThumbsDown,
  faUndo, faClipboardCheck

);

// Routing configuration
const routes: Routes = [
  { path: 'sign-in', component: SignInComponent, canActivate: [AuthNoGuard] },
  { path: 'forgot-password', component: ForgotPasswordComponent, canActivate: [AuthNoGuard] },
  { path: 'reset-password', component: ResetPasswordComponent }, // Logs the user out
  { path: 'confirm-email', component: ConfirmEmailComponent }, // Logs the user out
  { path: 'privacy-policy', component: PrivacyPolicyComponent },
  { path: 'terms-of-service', component: TermsOfServiceComponent },
  {
    path: 'client',
    component: ShellComponent,
    canActivate: [AuthGuard],
    canActivateChild: [AuthGuard],
    resolve: { globals: GlobalsResolverService },
    children: [
      { path: 'users', component: UsersComponent },
      { path: 'users/:id', component: UserDetailsComponent, canDeactivate: [DeactivateGuard] },
      { path: 'stores', component: StoresComponent },
      { path: 'stores/:id', component: StoreDetailsComponent, canDeactivate: [DeactivateGuard] },
      { path: 'products', component: ProductsComponent },
      { path: 'products/:id', component: ProductDetailsComponent, canDeactivate: [DeactivateGuard] },
      { path: 'support-requests', component: SupportRequestsComponent },
      { path: 'support-requests/:id', component: SupportRequestDetailsComponent, canDeactivate: [DeactivateGuard] },
      { path: 'data', component: DataComponent },
      { path: 'my-account', component: MyAccountComponent, canDeactivate: [DeactivateGuard]},
    ]
  },
  { path: '', redirectTo: 'client/support-requests', pathMatch: 'full' },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  declarations: [
    AppComponent,
    PageNotFoundComponent,
    SignInComponent,
    ForgotPasswordComponent,
    ConfirmEmailComponent,
    ResetPasswordComponent,
    PrivacyPolicyComponent,
    TermsOfServiceComponent,
    ShellComponent,
    UsersComponent,
    StoresComponent,
    ProductsComponent,
    SupportRequestsComponent,
    UserDetailsComponent,
    StoreDetailsComponent,
    ProductDetailsComponent,
    SupportRequestDetailsComponent,
    DataComponent,
    AuthComponent,
    MasterComponent,
    DetailsComponent,
    BrandComponent,
    SpinnerComponent,
    FormFieldComponent,
    DecimalEditorComponent,
    DetailsPickerComponent,
    ErrorMessageComponent,
    SuccessMessageComponent,
    ResetPasswordControlComponent,
    SerialPipe,
    MyAccountComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    RouterModule.forRoot(routes),
    FontAwesomeModule,
    NgbDropdownModule,
    NgbModalModule,
    NgbCollapseModule,
    NgbPopoverModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpRequestInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
