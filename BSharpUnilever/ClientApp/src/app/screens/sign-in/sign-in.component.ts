import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../data/auth.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'b-sign-in',
  templateUrl: './sign-in.component.html',
  styles: []
})
export class SignInComponent {

  public email: string;
  public password: string;
  public showSpinner = false;
  public errorMessage: string = null;

  constructor(private auth: AuthService) { }

  onSignIn() {
    this.showSpinner = true;
    this.errorMessage = null;
    this.auth.createToken(this.email, this.password).subscribe(
      () => {
        this.showSpinner = false;
      },
      (error: any) => {
        this.errorMessage = error;
        this.showSpinner = false;
      });
  }

  get currentYear(): number {
    return new Date().getFullYear();
  }
}
