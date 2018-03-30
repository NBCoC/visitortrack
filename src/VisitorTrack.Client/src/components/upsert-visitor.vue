<template>
  <div class="page">
    <div class="columns is-mobile">
      <div class="column"></div>
      <div class="column is-three-quarters-mobile">
        <div class="card">
            <form @submit.prevent="save">
              <header class="card-header">
                <p class="card-header-title">
                  {{ title }}
                </p>
              </header>
              <div class="card-content">
                <div class="content">
                 
                  <div class="field">
                    <label class="label">Full Name</label>
                    <div class="control">
                      <input class="input" type="text" placeholder="Full Name..." v-model="model.fullName">
                    </div>
                  </div>

                  <div class="field">
                    <label class="label">Status</label>
                    <div class="control">
                      <div class="select full-width">
                        <select v-model="model.statusId" class="full-width">
                          <option v-for="status in statusList" :key="status.id" :value="status.id">{{ status.name }}</option>
                        </select>
                      </div>
                    </div>
                  </div>

                  <div class="field">
                    <label class="label">Age Group</label>
                    <div class="control">
                      <div class="select full-width">
                        <select v-model="model.ageGroupId" class="full-width">
                          <option v-for="ageGroup in ageGroups" :key="ageGroup.id" :value="ageGroup.id">{{ ageGroup.name }}</option>
                        </select>
                      </div>
                    </div>
                  </div>

                  <div class="field">
                    <label class="label">Visit Date</label>
                    <div class="control">
                      <input type="text" ref="visitDateControl" class="input" placeholder="Visit Date...">
                    </div>
                  </div>

                  <div class="field">
                    <label class="label">Description</label>
                    <div class="control">
                      <textarea class="textarea" placeholder="Description..." v-model="model.description"></textarea>
                    </div>
                  </div>

                  <div class="field">
                    <button type="submit" class="button is-primary full-width" :class="{ 'is-loading' : isBusy }">
                      <span>
                        <i class="fa fa-save"></i> Save
                      </span>
                    </button>
                  </div>

                  <div class="field">
                    <button type="button" class="button is-danger full-width" :class="{ 'is-loading' : isBusy }" @click="remove" v-show="model.id">
                      <span>
                        <i class="fa fa-trash"></i> Delete
                      </span>
                    </button>
                  </div>
                </div>
              </div>
            </form>
          </div>
      </div>
      <div class="column"></div>
    </div>
  </div>
</template>
<script>
import {
  getVisitor,
  getAgeGroups,
  getStatusList,
  createVisitor,
  updateVisitor,
  deleteVisitor
} from '../api';

import { confirm, apiError } from '../bus';
import * as BulmaCalendar from 'bulma-calendar';

export default {
  created() {
    this.getAgeGroups();
    this.getStatusList();
    this.getModel();
  },
  mounted() {
    this.initDatePickers();
  },
  data() {
    return {
      model: { ageGroupId: 0, statusId: 0 },
      ageGroups: [],
      statusList: []
    };
  },
  props: ['id'],
  computed: {
    title() {
      return this.model.id ? 'Edit Visitor' : 'New Visitor';
    }
  },
  methods: {
    getModel() {
      const that = this;
      const id = that.$route.params.id;
      if (id) {
        getVisitor(that.token, id)
          .then(data => {
            that.model = data;
          })
          .catch(apiError);
      }
    },
    getAgeGroups() {
      const that = this;
      getAgeGroups(that.token)
        .then(data => {
          that.ageGroups = data;
        })
        .catch(apiError);
    },
    getStatusList() {
      const that = this;
      getStatusList(that.token)
        .then(data => {
          that.statusList = data;
        })
        .catch(apiError);
    },
    initDatePickers() {
      const that = this;
      new BulmaCalendar(that.$refs.visitDateControl, {
        startDate: new Date(),
        overlay: true,
        dateFormat: 'mm/dd/yyyy',
        onSelect: date => (that.model.firstVisitedOn = date)
      });
    },
    save() {
      const that = this;

      if (!that.model.fullName) return;

      const model = {
        fullName: that.model.fullName,
        ageGroupId: that.model.ageGroupId,
        statusId: that.model.statusId,
        description: that.model.description,
        firstVisitedOn: that.model.firstVisitedOn
      };

      that.isWorking = true;

      const upsert = that.model.id
        ? updateVisitor(that.token, that.user.id, that.model.id, model)
        : createVisitor(that.token, that.user.id, model);

      upsert
        .then(() => {
          that.isWorking = false;
          that.$router.push('/home');
        })
        .catch(error => {
          that.isWorking = false;
          apiError(error);
        });
    },
    remove() {
      const that = this;

      const callback = () => {
        that.isWorking = true;
        deleteVisitor(that.token, that.user.id, that.model.id)
          .then(() => {
            that.isWorking = false;
            that.$router.push('/home');
          })
          .catch(error => {
            that.isWorking = false;
            apiError(error);
          });
      };

      confirm(
        `Delete ${that.model.fullName}`,
        'Are you sure you want to delete this visitor?',
        callback
      );
    }
  }
};
</script>
<style scoped></style>


