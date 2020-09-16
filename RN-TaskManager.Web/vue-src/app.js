import layout from "./layouts/app-layout.vue";
import router from "./routers/app-router";


new Vue({
    vuetify: new Vuetify(),
    router,
    render: h => h(layout)
}).$mount("#app");
