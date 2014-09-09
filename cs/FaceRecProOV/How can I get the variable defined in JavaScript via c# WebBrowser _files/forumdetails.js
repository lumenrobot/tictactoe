
Forums.ForumDetails =
{
    
    wireAllBreadcrumbs: function () {
        $('.forumBreadcrumb').hover(function () {
            Forums.ForumDetails.getForumDetails($(this));
            var detailstip = $(this).find('.forumDetailsTip');
            detailstip && detailstip.show();
        }, function () {
            var detailstip = $(this).find('.forumDetailsTip');
            detailstip && detailstip.hide();
        });
    },
    
    wireForumItem: function () {
        var timer = null;
        $('.forumItem').not('.selectAllCheckboxContainer').hover(function () {
            Forums.ForumDetails.getForumDetails($(this));
            var detailstip = $(this).find('.forumDetailsTip');
            timer = setTimeout(function () {
                timer = null;
                detailstip && detailstip.show();
            }, 1000);
            
        }, function () {
            var detailstip = $(this).find('.forumDetailsTip');
            detailstip && detailstip.hide();
            if (timer) {
                clearTimeout(timer);
            }
        });
    },

    getForumDetails: function (forumDetailsParent) {
        var forumDetailsContainer = forumDetailsParent.find('.forumDetailsTip');

        var forumDetailsUrl = forumDetailsParent.data('forumdetailsurl');
        $.getJSON(forumDetailsUrl,
            function(data) {
                Forums.ForumDetails.loadForumDetails(data, forumDetailsContainer);
            })
            .fail(
                function(jqXHR, textStatus, err) {
                    forumDetailsParent.next('.forumDetailsTip').html('Error: ' + err);
                }
            );
    },

    loadForumDetails: function(data, forumDetailscontainer) {
        var val = data;
        
        var singleForumDetails = forumDetailscontainer.find('#forumDetailsTemplate');
        if (singleForumDetails.length > 0) {
            $(singleForumDetails).attr('id', 'forumDetailsTip_' + val.Name).removeClass("singleforumdetails");
            $(singleForumDetails).children('h3.header').prepend(val.DisplayName);
        } else {
            singleForumDetails = forumDetailscontainer.find('#forumDetailsTip_' + val.Name);
        }
        
        
        $(singleForumDetails).children('div.content').html(val.Description);
        $(singleForumDetails).find('img.rss').parent().attr('href', val.RSSUrl);
        $(singleForumDetails).find('.announcements span').text(val.AnnouncementsText + ': ' + val.AnnouncementCount).addClass("bold");
        
        if (val.ShowManageForums) {
            var manageForumsSection = $(singleForumDetails).find('.manageForumSection');
            $(manageForumsSection).find('.manageForumLink').attr('href', val.ManageForumUrl).text(val.ManageForumsText);
            $(manageForumsSection).find('.manageRolesLink').attr('href', val.ManageRolesUrl).text(val.ManageRolesText);
            $(manageForumsSection).find('.addAnnouncementLink').attr('href', val.AddAnnouncementUrl).text(val.AddAnnouncementText);
            $(manageForumsSection).show();
        }

        var favoriteUnfavoriteLink = $(singleForumDetails).find('a.addRemoveForum');
        $(favoriteUnfavoriteLink).attr('href', val.FavoriteUrl);
        if (val.HasFavoritePermission) {
            $(favoriteUnfavoriteLink).attr('title', val.AddToMyForumsText).attr('name', 'favorite').attr('data-reverseText', val.RemoveFromMyForumsText).attr('data-reversename', 'unfavorite').text(val.AddToMyForumsText);
        } else if (val.HasUnFavoritePermission === true) {
            $(favoriteUnfavoriteLink).attr('title', val.RemoveFromMyForumsText).attr('name', 'unfavorite').attr('data-reverseText', val.AddToMyForumsText).attr('data-reversename', 'favorite').text(val.RemoveFromMyForumsText);
        }

        var favoriteUnfavoriteLinkName = $(favoriteUnfavoriteLink).attr('name');
        if (typeof favoriteUnfavoriteLinkName !== 'undefined' && favoriteUnfavoriteLinkName !== false) {
            $(favoriteUnfavoriteLink).show();
        }

        $(forumDetailscontainer).find("a.addRemoveForum").click(function(evt) {
            var isFavorite = $(this).attr('name') === 'favorite';
            var link = $(this);

            $.post($(link).attr("href"), { isFavorited: isFavorite }, function(results) {
                if (results.success) {
                    var currentText = $(link).text();
                    var currentName = $(link).attr('name');
                    var reverseText = $(link).data('reversetext');
                    var reverseName = $(link).data('reversename');
                    $(link).attr('title', reverseText).attr('name', reverseName).attr('data-reversetext', currentText).attr('data-reversename', currentName);
                    $(link).text(reverseText);
                }
            }, "json");
            return false;
        });
        

    }
}