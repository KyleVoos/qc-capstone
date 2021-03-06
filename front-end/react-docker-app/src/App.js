import React from 'react';
import Layout from './Layout';
import { Route, Switch } from 'react-router';
import { SignInPage } from './components/SignInPage';
import { Home } from './components/Home'
import { SignUpPage } from './components/SignUpPage';
import { ForgotEmail } from './components/ForgotEmail';
import { ForgotPassword } from './components/ForgotPassword';
import { ProductOffer } from './components/ProductOffer';
import { ConfirmationPage } from './components/ConfirmationPage';
import { ManagePage } from './components/ManageUser'
import { SignIn_SignUp } from './components/SignIn_SignUp';
import { BrowsingList } from './components/BrowsingList';
import { Vendors } from './components/Vendors';
import { Cart } from './components/Cart2';
import { CheckOut } from './components/CheckOutPage';
import { SupplierPage } from './components/SupplierPage';
import { ContactUsPage } from './components/ContactUsPage';
export default () => (
    <Layout>
        <Switch>
            <Route exact path='/' component={Home} />
            <Route path='/signin' component={SignInPage} />
            <Route path='/signuppage' component={SignUpPage} />
            <Route path='/manageuser' component={ManagePage} />
            <Route path='/forgotemail' component={ForgotEmail} />
            <Route path='/forgotpassword' component={ForgotPassword} />
            <Route path="/supplier/:ids" component={SupplierPage} />
            <Route path="/vendors/:idv" component={Vendors} />
            <Route path='/browsing' component={BrowsingList} />
            <Route path='/confirmation' component={ConfirmationPage} />
            <Route path='/checkout' component={CheckOut} />
            <Route path='/cart' component={Cart} />
            <Route path='/contactus' component={ContactUsPage} />
            <Route path='/signinpage' component={SignIn_SignUp} />

            <Route path="/offering/:offerid" component={ProductOffer} />
       </Switch>
    </Layout>
    );