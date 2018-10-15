import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../data/auth.service';

@Component({
  selector: 'b-forgot-password',
  templateUrl: './forgot-password.component.html',
  styles: []
})
export class ForgotPasswordComponent implements OnInit {

  public email: string;
  public errorMessage: string;
  public successMessage: string;
  public showSpinner = false;

  constructor(private auth: AuthService) { }

  ngOnInit() {
  }

  onRequestResetLink() {
    this.errorMessage = null;
    this.successMessage = null;

    if (!this.email) {
      this.errorMessage = `Please enter your email`;
      return;
    }

    this.showSpinner = true;
    this.auth.forgotPassword(this.email).subscribe(
      () => {
        this.showSpinner = false;
        this.successMessage = `A password reset link has been sent to your email.
                               If you don't find it please check the spam folder`;
        this.email = null;
      },
      (err) => {
        this.showSpinner = false;
        this.errorMessage = err;
      }
    );
  }
}
