﻿import store from './store'
import layout from "./layouts/app-layout.vue";
import router from "./routers/app-router";
// Translation provided by Vuetify (typescript)
//import ru from 'vuetify/src/locale/ru' 

new Vue({
    store,
    vuetify: new Vuetify(),
    router,
    render: h => h(layout)
}).$mount("#app");

