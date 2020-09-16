import Router from "vue-router";
import notFound from '../not-found.vue';
import groups from "../components/groups";

Vue.use(Router);

export default new Router({
    routes: [
        //default route redirection
        {
            path: "/",
            redirect: { name: "groups" },
            display: false
        },
        //not found route redirection
        {  
            path: "*",
            component: notFound,
            display: false
        },
        { 
            path: "/not-found",
            name: "not Found",
            component: notFound,
            display: false
        },
        {
            path: "/groups",
            name: "groups",
            component: groups,
            display: true,
            label: "Группы",
            panel: "Справочники",
            icon: "mdi-account-group"
        },
        /*
        {
            path: "/document-uploader",
            name: "documentUploader",
            component: documentUploader,
            display: true,
            label: "Пакетная загрузка",
            panel: "Проекты",
            icon: "mdi-cloud-upload"
        },
        {
            path: "/sostav-editor",
            name: "sostavEditor",
            component: sostavEditor,
            display: false,
            label: "Редактор состава",
            panel: "Проекты"
        },
        {
            path: "/sostav-import",
            name: "sostavImport",
            component: sostavImport,
            display: true,
            label: "Импорт состава РД",
            panel: "Проекты",
            icon: "mdi-checkerboard"
        }
        */
    ]
});