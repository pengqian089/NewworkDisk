<template>
  <div class="container">
    <div class="code-box">
      <div class="box-header">
        <div class="details-container">
          <div class="desc">
            <a href="javascript:;" style="margin-right: 10px" v-on:click="createDir">新建文件夹</a>
            <input id="file" type="file" @change="upload" style="margin-right: 10px">
            <input type="text" id="txtSearch" v-model="searchText"/> <a href="javascript:;" v-on:click="search">搜索</a>
          </div>
          <div class="desc">
            {{ currentPath.join("/") }}
          </div>
        </div>
      </div>

      <div class="box-rows">
        <div class="row prev" v-if="!isRoot">
          <div class="header">
            <a href="javascript:;" v-on:click="goToPrev(prevUrl)"><span style="min-width: 16px">上一页</span></a>
          </div>
        </div>

        <div class="row" v-for="dir in directories" v-bind:todo="dir" v-bind:key="dir.name">
          <div class="icon">
            <svg height="16" class="git-dir" color="blue-3" aria-label="Directory" viewBox="0 0 16 16" width="16"
                 role="img">
              <path
                  d="M1.75 1A1.75 1.75 0 000 2.75v10.5C0 14.216.784 15 1.75 15h12.5A1.75 1.75 0 0016 13.25v-8.5A1.75 1.75 0 0014.25 3h-6.5a.25.25 0 01-.2-.1l-.9-1.2c-.33-.44-.85-.7-1.4-.7h-3.5z"></path>
            </svg>
          </div>
          <div class="header">
            <span>
                <a href="javascript:;" v-on:click="openDir(dir.name)" :title="dir.name">{{ dir.name }}</a>
            </span>
          </div>
          <div class="desc">
            <span>
                
            </span>
          </div>
          <div class="time">
            <a href="javascript:;" v-on:click="delDir(dir.name)" style="margin-right: 8px">删除</a>
            <a href="javascript:;" v-on:click="renameDir(dir.name)">重命名</a>
          </div>
        </div>


        <div class="row" v-for="file in files" v-bind:todo="file" v-bind:key="file.name">
          <div class="icon">
            <svg height="16" class="git-file" color="gray-light" aria-label="File" viewBox="0 0 16 16" width="16"
                 role="img">
              <path
                  d="M3.75 1.5a.25.25 0 00-.25.25v11.5c0 .138.112.25.25.25h8.5a.25.25 0 00.25-.25V6H9.75A1.75 1.75 0 018 4.25V1.5H3.75zm5.75.56v2.19c0 .138.112.25.25.25h2.19L9.5 2.06zM2 1.75C2 .784 2.784 0 3.75 0h5.086c.464 0 .909.184 1.237.513l3.414 3.414c.329.328.513.773.513 1.237v8.086A1.75 1.75 0 0112.25 15h-8.5A1.75 1.75 0 012 13.25V1.75z"></path>
            </svg>
          </div>
          <div class="header">
            <span>
                <a href="javascript:;" :title="file.name">{{ file.name }}</a>
            </span>
          </div>
          <div class="desc">
            <span>
                
            </span>
          </div>
          <div class="time">
            <a :href="'/network-disk/download?path=' + (currentPath.length === 0 ? '' : currentPath.join('/') + '/') + file.name + '&account=' + $store.getters.getAccount" target="_blank"
               style="margin-right: 8px">下载</a>
            <a href="javascript:;" v-on:click="delFile(currentPath,file.name)" style="margin-right: 8px">删除</a>
            <a href="javascript:;" v-on:click="rename(currentPath,file.name)">重命名</a>
          </div>
        </div>


      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      isRoot: false,
      directories: [],
      files: [],
      prevUrl: [],
      currentPath: [],
      path: [],
      searchText: "",
    };
  },
  mounted: function () {
    this.$nextTick(function () {
      console.log("start");
      let that = this;
      let p = this.path.join("/");
      this.request.fetch("/api/disk/myFolder", {path: p}).then(function (data) {
        that.isRoot = data.isRoot;
        that.directories = data.directories;
        that.files = data.files;
        that.prevUrl = data.prevUrl;
        that.currentPath = data.currentPath;
      });
    })
  },
  name: "NetworkDisk",
  methods: {
    reloadDir: function () {
      let p = this.path.join("/");
      let that = this;
      this.request.fetch("/api/disk/myFolder", {path: p}).then(function (data) {
        that.isRoot = data.isRoot;
        that.directories = data.directories;
        that.files = data.files;
        that.prevUrl = data.prevUrl;
        that.currentPath = data.currentPath;
      });
    },
    openDir: function (name) {
      this.path.push(name);
      this.reloadDir();
    },
    goToPrev: function (prev) {
      let that = this;
      this.request.fetch("/api/disk/myFolder", {path: prev.join("/")}).then(function (data) {
        that.isRoot = data.isRoot;
        that.directories = data.directories;
        that.files = data.files;
        that.prevUrl = data.prevUrl;
        that.currentPath = data.currentPath;
        that.path = prev;
      });
    },
    createDir: function () {
      let that = this;
      this.layer.prompt({
        title: "请输入文件夹名称", formType: 1
      }, function (pass, index) {
        let loadIndex = that.layer.loading();
        that.request.post("/api/disk/create",
            {
              path: that.path.join("/"),
              name: pass
            })
            .then(() => {
              that.reloadDir();
              that.layer.close(index);
              that.layer.close(loadIndex);
            });
      });
    },
    search: function () {
      this.$router.push({path: "search", query: {key: this.searchText}});
    },
    delFile: function (path, name) {
      let that = this;
      this.request.post("/api/disk/file", {path: path.join("/"), name: name}).then(() => {
        that.reloadDir();
      });
    },
    rename: function (path, fileName) {
      let that = this;
      this.layer.prompt({
        title: "请输入新名称", formType: 1
      }, function (pass, index) {
        let loadIndex = that.layer.loading();
        that.request.post("/api/disk/renameFile",
            {
              path: path.join("/"),
              fileName: fileName,
              newFileName: pass
            })
            .then(() => {
              that.reloadDir();
              that.layer.close(index);
              that.layer.close(loadIndex);
            });
      });
    },
    delDir: function (name) {
      let that = this;
      let p = this.currentPath;
      p.push(name);
      console.log(p);
      this.request.post("/api/disk/dir", {path: p.join("/")}).then(() => {
        that.reloadDir();
      });
    },
    renameDir: function (name) {
      let that = this;
      let p = this.currentPath;
      p.push(name);
      this.layer.prompt({
        title: "请输入新名称", formType: 1
      }, function (pass, index) {
        let loadIndex = that.layer.loading();
        that.request.post("/api/disk/RenameFolder",
            {
              path: p.join("/"),
              NewName: pass
            })
            .then(() => {
              that.reloadDir();
              that.layer.close(index);
              that.layer.close(loadIndex);
            });
      });
    },
    upload: function (event) {
      let that = this;
      let file = event.target.files[0];
      let param = new FormData();
      param.append('file', file);
      param.append("path", this.currentPath.join("/"));
      console.log(param.get('file'));
      let config = {
        headers: {'Content-Type': 'multipart/form-data'}
      };
      let index = this.layer.loading();
      this.$axios.post('/api/disk/upload', param, config)
          .then(() => {
            that.reloadDir();
            that.layer.close(index);
          })

    }
  }
}
</script>

<style scoped>
.container {
  width: 1170px;
  margin: 0 auto;
}

.git-dir {
  box-sizing: border-box;
  color: rgb(121, 184, 255);
  display: inline-block;
  fill: rgb(121, 184, 255);
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Helvetica, Arial, sans-serif, "Apple Color Emoji", "Segoe UI Emoji";
  font-size: 14px;
  height: 16px;
  line-height: 21px;
  list-style-type: none;
  overflow-wrap: break-word;
  overflow-x: hidden;
  overflow-y: hidden;
  text-size-adjust: 100%;
  vertical-align: text-bottom;
  width: 16px;
}

.git-file {
  box-sizing: border-box;
  color: rgb(106, 115, 125);
  display: inline-block;
  fill: rgb(106, 115, 125);
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Helvetica, Arial, sans-serif, "Apple Color Emoji", "Segoe UI Emoji";
  font-size: 14px;
  height: 16px;
  line-height: 21px;
  list-style-type: none;
  overflow-wrap: break-word;
  overflow-x: hidden;
  overflow-y: hidden;
  text-size-adjust: 100%;
  vertical-align: text-bottom;
  width: 16px;
}

.code-box {
  margin-top: 10px;
  background-color: #fff;
  border: 1px solid #e1e4e8;
  border-radius: 6px;
  box-sizing: border-box
}

.code-box .box-header {
  background-color: #f1f8ff;
  padding: 16px;
  margin: -1px -1px 0;
  border: 1px solid #c8e1ff;
  border-top-left-radius: 6px;
  border-top-right-radius: 6px;
  display: flex;
}

.code-box .box-header .details-container {
  align-items: center;
  display: flex;
}

.details-container .icon {
  margin: -4px !important;
  display: flex;
}

.details-container .desc {
  margin-left: 16px;
  min-width: 0;
  display: flex;
}

.code-box .box-rows {
  display: block;
  box-sizing: border-box;
  font-size: 14px;
  line-height: 1.5;
  color: #24292e;
}

.code-box .box-rows .row {
  display: flex;
  margin-top: -1px;
  list-style-type: none;
  border-top: 1px solid #e1e4e8;
  box-sizing: border-box;
  height: 38px;
  padding: 8px 16px;
}

.code-box .box-rows .row:hover {
  background-color: #f6f8fa;
}

.code-box .box-rows .row:last-of-type {
  border-bottom-right-radius: 6px;
  border-bottom-left-radius: 6px;
  border-bottom: 1px solid #e1e4e8;
}

.code-box .box-rows .row:last-child {
  border-bottom: 1px solid #e1e4e8;
}

.code-box .box-rows .row .prev {
}

.code-box .box-rows .row .icon {
  width: 16px;
  margin-right: 16px;
  box-sizing: border-box;
}

.code-box .box-rows .row .header {
  margin-right: 16px;
  min-width: 0;
  flex: auto;
}

.code-box .box-rows .row .header span {
  vertical-align: top;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  display: block;
  max-width: 100%;
  box-sizing: border-box;
}

.code-box .box-rows .row .header a {
  color: #24292e;
  text-decoration: none;
  background-color: initial;
  box-sizing: border-box
}

.code-box .box-rows .row .header a:hover {
  color: #0366d6;
  text-decoration: underline;
  outline-width: 0;
}

.code-box .box-rows .row .desc {
  display: block;
  margin-right: 16px;
  min-width: 0;
  flex: auto;
  width: 41.66667%;
  box-sizing: border-box;
}

.code-box .box-rows .row .desc span {
  vertical-align: top;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  display: block;
  max-width: 100%;
  box-sizing: border-box;
}

.code-box .box-rows .row .time {
  width: 120px;
  text-align: right;
  color: #6a737d;
}

.code-box .box-rows pre {
  font-weight: bold
}

</style>