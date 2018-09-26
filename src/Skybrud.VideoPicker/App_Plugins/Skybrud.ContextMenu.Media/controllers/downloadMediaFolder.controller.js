angular.module("umbraco").controller("Skybrud.Editors.Media.DownloadMediaFolderController",
    function($scope, $window) {
        var dialogOptions = $scope.dialogOptions;
        var node = dialogOptions.currentNode;


        $scope.recursive = false;

        $scope.download = function() {
            $window.open('/umbraco/backoffice/DownloadMediaFolder/DownloadMediaFolder/GetMedia?mediaFolderId=' + node.id + '&recursive=' + $scope.recursive);

            nav.hideDialog();
        };
    });