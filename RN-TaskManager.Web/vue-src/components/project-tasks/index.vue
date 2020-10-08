<template>
    <div class="component-container">

        <v-toolbar flat color="white">
            <v-toolbar-title>{{title}} [{{countItems}}]</v-toolbar-title>

            <v-divider class="mx-3"
                       inset
                       vertical></v-divider>

            <v-btn @click="onChangeMyTask" class="" small>
                <v-icon small
                        :color="filters.myTask ? 'green' : 'grey'"
                        class="mr-2">
                    mdi-brightness-1
                </v-icon>
                Мои задачи
            </v-btn>

            <v-btn @click="onChangeFilterWeek" class="ml-2" title="Нет окончания факта и окончание плановое попадает на текущую неделю" small>
                <v-icon small
                        :color="filters.week ? 'green' : 'grey'"
                        class="mr-2">
                    mdi-brightness-1
                </v-icon>
                На неделю
            </v-btn>

            <v-divider class="mx-3"
                       inset
                       vertical></v-divider>

            <v-btn @click="onChangeView('table')" class="" title="В виде сплошной таблицы" small color="blue lighten-5">
                <v-icon small
                        :color="views.table ? 'green' : 'grey'"
                        class="mr-2">
                    mdi-brightness-1
                </v-icon>
                Таблица
            </v-btn>

            <v-btn @click="onChangeView('groupedByGroup')" class="ml-2" title="Сгруппировать по группе и исполнителю" small color="blue lighten-5">
                <v-icon small
                        :color="views.groupedByGroup ? 'green' : 'grey'"
                        class="mr-2">
                    mdi-brightness-1
                </v-icon>
                Группа-Исполнитель
            </v-btn>

            <v-btn @click="onChangeView('groupedByProject')" class="ml-2" title="Сгруппировать по проекту" small color="blue lighten-5">
                <v-icon small
                        :color="views.groupedByProject ? 'green' : 'grey'"
                        class="mr-2">
                    mdi-brightness-1
                </v-icon>
                Проект
            </v-btn>

            <v-spacer></v-spacer>

            <v-dialog v-model="dialog" max-width="800px" persistent>
                <template v-slot:activator="{ on, attrs }">
                    <v-btn color="primary"
                           dark
                           class=""
                           v-bind="attrs"
                           v-on="on">Добавить запись</v-btn>
                </template>
                <v-card>
                    <v-card-title>
                        <v-tooltip top>
                            <template v-slot:activator="{ on, attrs }">
                                <v-icon color="blue" v-bind="attrs" v-on="on">mdi-details</v-icon>
                            </template>
                            <span>
                                {{getDetailsString('Создано ', editedItem.LoginCreated, editedItem.DateCreated)}}<br />
                                {{getDetailsString('Изменено ', editedItem.LoginEdited, editedItem.DateEdited)}}<br />
                                {{getDetailsString('Удалено ', editedItem.LoginDeleted, editedItem.DateDeleted)}}
                            </span>
                        </v-tooltip>

                        <span class="headline ml-2">Карточка задачи</span>
                        <v-spacer></v-spacer>
                        <v-btn fab
                               small
                               depressed
                               @click="close">
                            <v-icon>
                                mdi-close
                            </v-icon>
                        </v-btn>
                    </v-card-title>

                    <v-card-text class="card-body">
                        <v-container>
                            <v-row>
                                <v-col cols="12">
                                    <v-textarea single-line label="Описание работы" v-model="editedItem.Details" rows="3" dense></v-textarea>
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col cols="5">
                                    <v-autocomplete :items="projects"
                                                    item-text="ProjectName"
                                                    item-value="ProjectId"
                                                    v-model="editedItem.ProjectId"
                                                    :rules="[e => e > 0]"
                                                    dense
                                                    label="Проект"></v-autocomplete>
                                </v-col>
                                <v-col>
                                    <v-autocomplete :items="taskTypes"
                                                    item-text="TaskTypeName"
                                                    item-value="TaskTypeId"
                                                    v-model="editedItem.TaskTypeId"
                                                    :rules="[e => e > 0]"
                                                    dense
                                                    label="Тип задачи"></v-autocomplete>
                                </v-col>
                                <v-col cols="3">
                                    <v-select :items="statuses"
                                              item-text="StatusName"
                                              item-value="ProjectTaskStatusId"
                                              v-model="editedItem.ProjectTaskStatusId"
                                              :rules="[e => e > 0]"
                                              dense
                                              label="Статус"></v-select>
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col cols="5">
                                    <v-select :items="groups"
                                              item-text="GroupName"
                                              item-value="GroupId"
                                              v-model="editedItem.GroupId"
                                              :rules="[e => e > 0]"
                                              dense
                                              label="Группа"></v-select>
                                </v-col>

                                <v-col>
                                    <v-autocomplete v-model="editedItem.Users"
                                                    :items="users"
                                                    item-text="ShortName"
                                                    item-value="UserId"
                                                    dense
                                                    single-line
                                                    label="Исполнители"
                                                    multiple></v-autocomplete>
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col>
                                    <datePicker v-model="editedItem.StartPlan" label="Плановое начало"></datePicker>
                                </v-col>
                                <v-col>
                                    <datePicker v-model="editedItem.EndPlan" label="Плановое окончание"></datePicker>
                                </v-col>
                                <v-col cols="1">
                                </v-col>
                                <v-col cols="3">
                                    <v-text-field v-model="editedItem.Priority"
                                                  label="Приоритет"
                                                  type="number"
                                                  dense
                                                  :rules="[e => e >= 0 ]">
                                    </v-text-field>
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col>
                                    <datePicker v-model="editedItem.StartFact" label="Фактическое начало"></datePicker>
                                </v-col>
                                <v-col>
                                    <datePicker v-model="editedItem.EndFact" label="Фактическое окончание" v-on:input="onChangeEndFact"></datePicker>
                                </v-col>
                                <v-col cols="1">
                                </v-col>
                                <v-col cols="3">
                                    <v-text-field v-model="editedItem.DurationHours"
                                                  label="Трудозатраты чел/час"
                                                  type="number"
                                                  dense
                                                  :rules="[e => e >= 0 ]">
                                    </v-text-field>
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col>
                                    <v-textarea single-line label="Примечание" v-model="editedItem.Note" rows="5" dense class="font-small"></v-textarea>
                                </v-col>
                            </v-row>

                            <v-row>
                                <v-col>
                                    <v-select v-model="editedItem.BlockId"
                                              :items="blocks"
                                              item-text="BlockName"
                                              item-value="BlockId"
                                              dense
                                              clearable
                                              label="Блок"></v-select>
                                </v-col>

                                <v-col>
                                    <v-text-field v-model="editedItem.EffectBeforeHours"
                                                  label="Трудоемкость до автоматизации"
                                                  type="number"
                                                  dense
                                                  :rules="[e => e >= 0 ]">
                                    </v-text-field>
                                </v-col>

                                <v-col>
                                    <v-text-field v-model="editedItem.EffectAfterHours"
                                                  label="Трудоемкость после автоматизации"
                                                  type="number"
                                                  dense
                                                  :rules="[e => e >= 0 ]">
                                    </v-text-field>
                                </v-col>
                            </v-row>


                        </v-container>
                    </v-card-text>

                    <v-card-actions>
                        <v-btn color="red darken-1" text @click="deleteItem">Удалить</v-btn>
                        <v-spacer></v-spacer>
                        <v-btn color="blue darken-1" text @click="close">Отмена</v-btn>
                        <v-btn color="blue darken-1"
                               text @click="save"
                               :loading="loadings.save"
                               :disabled="loadings.save">Сохранить</v-btn>
                    </v-card-actions>
                </v-card>
            </v-dialog>
        </v-toolbar>

        <v-divider></v-divider>

        <div class="d-flex mt-2 toolbar-filters">

            <v-text-field v-model="search"
                          class=""
                          style="max-width: 15rem;"
                          label="Поиск"
                          outlined
                          dense
                          clearable></v-text-field>

            <v-select v-model="filters.status"
                      class="ml-2"
                      style="max-width: 12rem;"
                      :items="statuses"
                      item-text="StatusName"
                      item-value="ProjectTaskStatusId"
                      label="Статус"
                      outlined
                      dense
                      multiple>
                <template v-slot:selection="{ item, index }">
                    <span class="grey--text caption" v-if="index === 0">Выбрано {{ filters.status.length }}</span>
                </template>
            </v-select>

            <v-select v-model="filters.projects"
                      class="ml-2"
                      style="max-width: 15rem;"
                      :items="projects"
                      item-text="ProjectName"
                      item-value="ProjectId"
                      label="Проект"
                      outlined
                      dense
                      multiple>
                <template v-slot:selection="{ item, index }">
                    <span class="grey--text caption" v-if="index === 0">Выбрано {{ filters.projects.length }}</span>
                </template>
            </v-select>

            <v-select v-model="filters.groups"
                      class="ml-2"
                      style="max-width: 10rem;"
                      :items="groups"
                      item-text="GroupName"
                      item-value="GroupId"
                      label="Группа"
                      outlined
                      dense
                      multiple>
                <template v-slot:selection="{ item, index }">
                    <span class="grey--text caption" v-if="index === 0">Выбрано {{ filters.groups.length }}</span>
                </template>
            </v-select>

            <v-select v-model="filters.performers"
                      class="ml-2"
                      style="max-width: 15rem;"
                      :items="users"
                      item-text="ShortName"
                      item-value="UserId"
                      label="Исполнитель"
                      outlined
                      dense
                      multiple>
                <template v-slot:selection="{ item, index }">
                    <span class="grey--text caption" v-if="index === 0">Выбрано {{ filters.performers.length }}</span>
                </template>
            </v-select>


        </div>

        <v-divider></v-divider>

        <flatTable v-if="views.table"
                   :items="tasks"
                   :loading="loadings.firstLoad"
                   @select="editItem"></flatTable>

        <groupItem v-else-if="views.groupedByGroup"
                   :title="group.GroupName" class="level-1" v-for="group in filtered_groups" :key="'groupItem_group_' + group.GroupId">
            <groupItem :title="user.ShortName" class="level-2" v-for="user in users.filter(e => e.GroupId === group.GroupId)" :key="'groupItem_user_' + user.UserId">
                <flatTable :items="tasks.filter(e => e.PerformerIds.indexOf(user.UserId) !== -1)"
                           :loading="loadings.firstLoad"
                           :excludeHeaders="['GroupName', 'Performers']"
                           @select="editItem"></flatTable>
            </groupItem>
        </groupItem>

        <groupItem v-else-if="views.groupedByProject"
                   :title="project.ProjectName" class="level-1" v-for="project in filtered_projects" :key="'groupItem_project_' + project.ProjectId">
                <flatTable :items="tasks.filter(e => e.ProjectId === project.ProjectId)"
                           :loading="loadings.firstLoad"
                           :excludeHeaders="['ProjectName']"
                           @select="editItem"></flatTable>
        </groupItem>

        <v-snackbar v-model="snackbar">
            {{ errorMessage }}

            <template v-slot:action="{ attrs }">
                <v-btn color="error"
                       text
                       v-bind="attrs"
                       @click="closeSnackbar">
                    Закрыть
                </v-btn>
            </template>
        </v-snackbar>
    </div>
</template>

<script>

    import { localeDateFormat, ISODateFormat } from '../../helpers/date-helpers'

    import datePicker from './date-picker.vue'
    import flatTable from './flat-table.vue'
    import groupItem from './group-item.vue'

    const taskTemplate = {
        ProjectTaskId: 0,
        ProjectId: 0,
        ProjectTaskStatusId: 3,
        TaskTypeId: 0,
        GroupId: 0,
        Details: '',
        Note: '',
        StartPlan: new Date(Date.now()).toISOString(),
        EndPlan: '',
        StartFact: '',
        EndFact: '',
        DurationHours: 0,
        Priority: 0,
        Users: [],
        EffectBeforeHours: 0,
        EffectAfterHours: 0,
        BlockId: '',
    };

    export default {
        data: function () {
            return {
                title: 'Задачи',
                constants: {
                    statusInWorkIndex: 3,
                    statusEndIndex: 4,
                },
                dialog: false,
                snackbar: false,
                errorMessage: '',
                items: [],
                countItems: 0,
                projects: [],
                groups: [],
                statuses: [],
                taskTypes: [],
                users: [],
                blocks: [],
                editedIndex: -1,
                editedItem: Object.assign({}, taskTemplate),
                loadings: {
                    firstLoad: false,
                    items: false,
                    performers: false,
                    taskTypes: false,
                    save: false,
                },
                filters: {
                    status: [],
                    performers: [],
                    projects: [],
                    groups: [],
                    myTask: true,
                    week: false
                },
                views: {
                    table: true,
                    groupedByGroup: false,
                    groupedByProject: false
                },
                search: '',
                api: '/api/projectTasks'
            }
        },
        created() {
            this.loadItems();
        },
        computed: {
            tasks() {

                let items = this.items;

                // фильтр за неделю....
                if (this.filters.week) {

                    let currentDate = new Date;
                    var first = currentDate.getDate() - currentDate.getDay() + 1;
                    var last = first + 6;

                    var firstday = new Date(currentDate.setDate(first)).getTime();
                    var lastday = new Date(currentDate.setDate(last)).getTime();

                    items = items.filter(e => {

                        let pass = false;
                        let dStart = null;
                        let dEnd = null;

                        if (e.StartPlan !== null) {
                            dStart = new Date(e.StartPlan).getTime();

                            if (e.StartFact !== null)
                                dStart = new Date(e.StartPlan).getTime();
                        }

                        if (e.EndPlan !== null) {
                            dEnd = new Date(e.EndPlan).getTime();

                            if (e.EndFact !== null) {
                                //dEnd = new Date(e.EndFact).getTime();
                                return false;
                            }
                        }


                        if (dStart !== null && dEnd !== null)
                            pass = (dStart <= firstday && dEnd >= firstday) || (dStart >= firstday && dStart <= lastday) || (dStart <= firstday && dEnd <= firstday);

                        return pass;
                    });
                }

                // фильтр по статусу
                if (this.filters.status.length > 0)
                    items = items.filter(e => this.filters.status.indexOf(e.ProjectTaskStatusId) > -1);

                // фильтр по проекту
                if (this.filters.projects.length > 0)
                    items = items.filter(e => this.filters.projects.indexOf(e.ProjectId) > -1);

                // фильтр по группе
                if (this.filters.groups.length > 0)
                    items = items.filter(e => this.filters.groups.indexOf(e.GroupId) > -1);

                // фильтр по исполнителю
                if (this.filters.performers.length > 0)
                    items = items.filter(e => {

                        let users = e.Users;
                        let pass = false;

                        for (let user of users) {
                            if (this.filters.performers.indexOf(user) > -1) {
                                pass = true;
                                break;
                            }
                        }

                        return pass;
                    });

                // поиск
                items = items.filter(e => this.searchItem(e));

                this.countItems = items.length;

                return items;
            },
            filtered_groups() {

                let items = this.groups;
                let filters = this.filters.groups;

                if (filters.length !== 0) {
                    items = items.filter(e => filters.indexOf(e.GroupId) !== -1);
                }

                return items;
            },
            filtered_projects() {

                let items = this.projects;
                let filters = this.filters.projects;

                if (filters.length !== 0) {
                    items = items.filter(e => filters.indexOf(e.ProjectId) !== -1);
                }

                return items;
            },
        },
        methods: {
            fetchData(uri, callback) {

                let self = this;

                return fetch(uri, {
                    credentials: 'include',
                    accept: 'application/json'
                })
                    .then(response => {
                        if (response.status !== 200) {
                            console.log(response)
                            throw new Error(response.status + " - " + response.statusText)
                        }
                        else
                            return response.json()
                    })
                    .then(data => {
                        callback(data);
                        return data;
                    })
                    .catch(ex => {
                        console.log(uri);
                        console.log(ex);
                        self.errorMessage = ex;
                        self.snackbar = true;
                    });
            },
            async loadTasks() {
                let self = this;
                this.loadings.users = true;

                let uri = this.api;

                if (this.filters.myTask) {
                    uri += '/my';
                }

                await this.fetchData(uri, (data) => {
                    self.items = data.map(e => {

                        e.Users = e.PerformerIds;

                        return e;
                    });

                    self.loadings.users = false;
                });
            },
            async loadItems() {

                this.loadings.firstLoad = true;

                let self = this;

                await this.fetchData('/api/projects', (data) => {
                    self.projects = self.sortArray(data, 'ProjectName', 'string');
                });

                await this.fetchData('/api/groups', (data) => {
                    self.groups = self.sortArray(data, 'GroupName', 'string');
                });

                await this.fetchData('/api/projectTaskStatuses', (data) => {
                    self.statuses = data;
                    // фильтр по умолчанию - В работе и В обработке
                    self.filters.status = [2, 3];
                });

                await this.fetchData('/api/users', (data) => {
                    self.users = self.sortArray(data, 'ShortName', 'string');
                });

                await this.fetchData('/api/taskTypes', (data) => {
                    self.taskTypes = self.sortArray(data, 'TaskTypeName', 'string');
                });

                await this.fetchData('/api/blocks', (data) => {
                    self.blocks = self.sortArray(data, 'BlockName', 'string');
                });

                await this.loadTasks();

                this.loadings.firstLoad = false;

                await this.openTaskFromRoute();
            },
            async openTaskFromRoute() {
                let self = this;

                let taskId = parseInt(this.$route.params.task_id, 0);

                // открываем карточку указанной задачи
                if (taskId > 0) {
                    let selectedItems = this.items.filter(e => e.ProjectTaskId === taskId);

                    if (selectedItems.length > 0)
                        this.editItem(selectedItems[0]);
                    else {
                        // в текущем списке нет задачи...запрашиваем с серва?
                        let uri = this.api + '/' + taskId;
                        await this.fetchData(uri, (data) => {

                            data.Users = data.PerformerIds;

                            self.editedItem = data;
                            self.editedIndex = -2;
                            this.dialog = true;
                        });
                    }
                }
            },
            editItem(item) {

                this.editedIndex = this.items.indexOf(item);
                this.editedItem = Object.assign({}, item);

                if (parseInt(this.$route.params.task_id, 0) !== this.editedItem.ProjectTaskId)
                    this.$router.replace({ name: "project-task", params: { task_id: this.editedItem.ProjectTaskId }, query: {  } })

                this.dialog = true;
            },
            async deleteItem() {

                if (!confirm("Удалить выбранную запись?")) return;

                let self = this;
                let item = this.editedItem;

                await fetch(this.api + '/' + item.ProjectTaskId, {
                    method: 'DELETE',
                    credentials: 'include',
                })
                    .then(async (response) => {

                        if (response.ok) {

                            if (this.editedIndex !== -2 && this.editedIndex > -1)
                                this.items.splice(this.editedIndex, 1);

                            this.close();
                        }
                        else {
                            if (response.status === 400) {
                                self.errorMessage = await response.text();
                                self.snackbar = true;
                            }
                            else if (response.status === 404) {
                                self.errorMessage = "Запись не найдена";
                                self.snackbar = true;
                            }
                        }

                    });
            },
            close() {
                this.dialog = false;

                this.$nextTick(() => {
                    this.editedItem = Object.assign({}, taskTemplate);
                    this.editedIndex = -1;
                    this.$router.push({ name: "project-tasks" });
                })
            },
            async save() {

                this.loadings.save = true;

                let self = this;
                let needClose = false;
                let method = 'POST';
                const formData = new FormData();

                formData.append('ProjectId', this.editedItem.ProjectId);
                formData.append('ProjectTaskStatusId', this.editedItem.ProjectTaskStatusId);
                formData.append('TaskTypeId', this.editedItem.TaskTypeId);
                formData.append('GroupId', this.editedItem.GroupId);

                formData.append('StartPlan', ISODateFormat(this.editedItem.StartPlan));
                formData.append('StartFact', ISODateFormat(this.editedItem.StartFact));

                formData.append('EndPlan', ISODateFormat(this.editedItem.EndPlan));
                formData.append('EndFact', ISODateFormat(this.editedItem.EndFact));

                formData.append('Details', this.editedItem.Details);
                formData.append('Note', this.editedItem.Note);
                formData.append('DurationHours', this.editedItem.DurationHours);
                formData.append('Priority', this.editedItem.Priority);

                formData.append('Users', this.editedItem.Users);

                formData.append('BlockId', this.editedItem.BlockId !== undefined ? this.editedItem.BlockId : 0);
                formData.append('EffectAfterHours', this.editedItem.EffectAfterHours);
                formData.append('EffectBeforeHours', this.editedItem.EffectBeforeHours);

                if (this.editedIndex > -1 || this.editedIndex == -2 ) {
                    method = 'PUT';
                    formData.append('ProjectTaskId', this.editedItem.ProjectTaskId);
                }

                await fetch(this.api, {
                    method: method,
                    credentials: 'include',
                    body: formData
                })
                    .then(async (response) => {

                        self.loadings.save = false;

                        if (response.ok) {

                            const item = await response.json();

                            if (item.Users !== null)
                                item.Users = item.Users.split(',').map(s => parseInt(s, 0));

                            if (this.editedIndex > -1)
                                Object.assign(self.items[this.editedIndex], item);
                            else if(this.editedIndex !== -2)
                                self.items.push(item);

                            needClose = true;
                        }
                        else {
                            if (response.status === 400) {
                                self.errorMessage = await response.text();
                                self.snackbar = true;
                            }
                        }

                    });

                if (needClose)
                    this.close()
            },
            closeSnackbar() {
                this.snackbar = false;
                this.errorMessage = '';
            },

            tasksByUserId(userId) {
                return this.tasks.filter(e => e.PerformerIds.indexOf(userId));
            },

            getDetailsString(label, login, dateChanged) {

                if (login !== null)
                    return label + localeDateFormat(dateChanged) + ' (' + login + ')';
                else
                    return label + ' - нет информации';

            },
            
            searchItem(e) {

                if (this.search === null || this.search === '' || this.search.length < 2)
                    return true;

                if (e.Details !== null && e.Details.toLocaleLowerCase().includes(this.search.toLocaleLowerCase()))
                    return true;

                if (e.ProjectName.toLocaleLowerCase().includes(this.search.toLocaleLowerCase()))
                    return true;

                if (e.Performers.toLocaleLowerCase().includes(this.search.toLocaleLowerCase()))
                    return true;

                if (e.Group.GroupName.toLocaleLowerCase().includes(this.search.toLocaleLowerCase()))
                    return true;

                return false;
            },
            sortArray(items, fieldName, fieldType) {
                items.sort((a, b) => {
                    if (fieldType === 'number')
                        return a[fieldName] - b[fieldName];
                    else if (fieldType === 'string')
                        return ('' + a[fieldName]).localeCompare(b[fieldName]);
                });

                return items;
            },

            onChangeMyTask() {
                this.filters.myTask = !this.filters.myTask;
                this.loadTasks();
            },
            onChangeFilterWeek() {
                this.filters.week = !this.filters.week;
            },
            onChangeEndFact(e) {
                if (e !== '')
                    this.editedItem.ProjectTaskStatusId = this.constants.statusEndIndex;
            },
            onChangeView(e) {
                Object.keys(this.views).map(v => this.views[v] = false);
                this.views[e] = true;
            },
        },
        components: {
            datePicker,
            flatTable,
            groupItem
        }
    };
</script>

<style>

    .font-small textarea {
        font-size: 14px;
        line-height: 16px !important;
        padding-top: .5rem;
    }

    .col {
        padding: .15rem !important;
    }

    .toolbar-filters .v-input {
        height: 48px;
    }

    .card-body {
        padding-bottom: 0px !important;
    }


</style>