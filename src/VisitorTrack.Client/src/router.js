import Vue from 'Vue';
import VueRouter from 'VueRouter';
import Home from './components/home';
import SignIn from './components/sign-in';
import Search from './components/search';
import Users from './components/users';

Vue.use(VueRouter);

export default new VueRouter({
  routes: [
    {
      path: '/',
      redirect: 'home'
    },
    {
      path: '/home',
      name: 'home',
      component: Home
    },
    {
      path: '/sign-in',
      component: SignIn
    },
    {
      path: '/search',
      component: Search
    },
    {
      path: '/users',
      component: Users,
      meta: { adminView: true }
    },
    {
      path: '*',
      redirect: 'home'
    }
  ]
});
