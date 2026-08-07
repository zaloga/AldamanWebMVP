namespace Aldaman.Services.Resources;

/// <summary>
/// Constants for UI resource keys to ensure type safety.
/// </summary>
public static class UIResourceKeys
{
    // Layout & Navigation
    public const string Home = nameof(Home);
    public const string Logout = nameof(Logout);
    public const string Login = nameof(Login);
    public const string Admin = nameof(Admin);
    public const string Search = nameof(Search);

    // Account & Auth
    public const string AccessDenied = nameof(AccessDenied);
    public const string AccessDeniedMessage = nameof(AccessDeniedMessage);
    public const string BackToHome = nameof(BackToHome);
    public const string WelcomeToAdmin = nameof(WelcomeToAdmin);
    public const string Email = nameof(Email);
    public const string EmailOrPhone = nameof(EmailOrPhone);
    public const string Password = nameof(Password);
    public const string RememberMe = nameof(RememberMe);
    public const string ForgotPassword = nameof(ForgotPassword);
    public const string SignInButton = nameof(SignInButton);

    // Blog
    public const string Blog = nameof(Blog);
    public const string BlogSubtitle = nameof(BlogSubtitle);
    public const string AllArticles = nameof(AllArticles);
    public const string PreviousPost = nameof(PreviousPost);
    public const string NextPost = nameof(NextPost);
    public const string PostCreatedSuccessfully = nameof(PostCreatedSuccessfully);
    public const string ErrorCreatingPost = nameof(ErrorCreatingPost);
    public const string PostUpdatedSuccessfully = nameof(PostUpdatedSuccessfully);
    public const string ErrorUpdatingPost = nameof(ErrorUpdatingPost);
    public const string PostDeletedSuccessfully = nameof(PostDeletedSuccessfully);
    public const string ErrorDeletingPost = nameof(ErrorDeletingPost);
    public const string PostRestoredSuccessfully = nameof(PostRestoredSuccessfully);
    public const string ErrorRestoringPost = nameof(ErrorRestoringPost);
    public const string PostPermanentlyDeleted = nameof(PostPermanentlyDeleted);
    public const string ErrorPermanentlyDeletingPost = nameof(ErrorPermanentlyDeletingPost);

    // Contact
    public const string Contact = nameof(Contact);
    public const string ContactMe = nameof(ContactMe);
    public const string ContactSubtitle = nameof(ContactSubtitle);
    public const string Subject = nameof(Subject);
    public const string Message = nameof(Message);
    public const string SendMessage = nameof(SendMessage);
    public const string MessageSent = nameof(MessageSent);
    public const string ThankYouForMessage = nameof(ThankYouForMessage);
    public const string MessageSentConfirmation = nameof(MessageSentConfirmation);

    // Common & Errors
    public const string Error = nameof(Error);
    public const string ErrorOccurred = nameof(ErrorOccurred);
    public const string RequestId = nameof(RequestId);
    public const string SectionNoContent = nameof(SectionNoContent);
    public const string GenerateSlug = nameof(GenerateSlug);
    public const string TranslationMissing = nameof(TranslationMissing);
    public const string CreatedSuccessfully = nameof(CreatedSuccessfully);
    public const string ErrorCreating = nameof(ErrorCreating);
    public const string UpdatedSuccessfully = nameof(UpdatedSuccessfully);
    public const string ErrorUpdating = nameof(ErrorUpdating);
    public const string DeletedSuccessfully = nameof(DeletedSuccessfully);
    public const string ErrorDeleting = nameof(ErrorDeleting);
    public const string RestoredSuccessfully = nameof(RestoredSuccessfully);
    public const string ErrorRestoring = nameof(ErrorRestoring);
    public const string ConfirmRestoreText = nameof(ConfirmRestoreText);
    public const string PermanentlyDeleted = nameof(PermanentlyDeleted);
    public const string ErrorPermanentlyDeleting = nameof(ErrorPermanentlyDeleting);
    public const string Yes = nameof(Yes);
    public const string FileSizeExceedsLimit = nameof(FileSizeExceedsLimit);
    public const string ImageUploadFailed = nameof(ImageUploadFailed);
    public const string ErrorUploadingImage = nameof(ErrorUploadingImage);
    public const string Save = nameof(Save);
    public const string Edit = nameof(Edit);
    public const string Actions = nameof(Actions);
    public const string Restore = nameof(Restore);
    public const string DeletePermanently = nameof(DeletePermanently);

    // Style Settings
    public const string StyleSettings = nameof(StyleSettings);
    public const string CustomizeThemeVariables = nameof(CustomizeThemeVariables);
    public const string DeletedSettings = nameof(DeletedSettings);
    public const string ChooseColor = nameof(ChooseColor);
    public const string Key = nameof(Key);
    public const string Type = nameof(Type);
    public const string Value = nameof(Value);
    public const string StyleSettingUpdated = nameof(StyleSettingUpdated);
    public const string StyleSettingCreated = nameof(StyleSettingCreated);
    public const string ResetToDefault = nameof(ResetToDefault);
    public const string StyleSettingResetSuccessfully = nameof(StyleSettingResetSuccessfully);

    // Pages & Media
    public const string PageCreatedSuccessfully = nameof(PageCreatedSuccessfully);
    public const string ErrorCreatingPage = nameof(ErrorCreatingPage);
    public const string PageUpdatedSuccessfully = nameof(PageUpdatedSuccessfully);
    public const string ErrorUpdatingPage = nameof(ErrorUpdatingPage);
    public const string PleaseSelectFile = nameof(PleaseSelectFile);
    public const string FileUploadedSuccessfully = nameof(FileUploadedSuccessfully);
    public const string ErrorUploadingFile = nameof(ErrorUploadingFile);
    public const string AltText = nameof(AltText);
    public const string MediaMetadataUpdated = nameof(MediaMetadataUpdated);

    // Admin Common & Layout
    public const string Create = nameof(Create);
    public const string Delete = nameof(Delete);
    public const string Back = nameof(Back);
    public const string BackToList = nameof(BackToList);
    public const string Filter = nameof(Filter);
    public const string Clear = nameof(Clear);
    public const string Ascending = nameof(Ascending);
    public const string Descending = nameof(Descending);
    public const string Dashboard = nameof(Dashboard);
    public const string GoToWeb = nameof(GoToWeb);
    public const string AdminUser = nameof(AdminUser);
    public const string SignOut = nameof(SignOut);
    public const string Close = nameof(Close);
    public const string Success = nameof(Success);
    public const string Information = nameof(Information);
    public const string PleaseCorrectErrors = nameof(PleaseCorrectErrors);
    public const string Details = nameof(Details);
    public const string State = nameof(State);
    public const string NoItemsFound = nameof(NoItemsFound);
    public const string None = nameof(None);
    public const string Title = nameof(Title);
    public const string Content = nameof(Content);
    public const string Translations = nameof(Translations);
    public const string Metadata = nameof(Metadata);
    public const string SystemInformation = nameof(SystemInformation);
    public const string Id = nameof(Id);
    public const string Created = nameof(Created);
    public const string LastModified = nameof(LastModified);
    public const string DeletedOn = nameof(DeletedOn);
    public const string Deleted = nameof(Deleted);
    public const string Manage = nameof(Manage);
    public const string Cancel = nameof(Cancel);

    // Pagination
    public const string Showing = nameof(Showing);
    public const string To = nameof(To);
    public const string Of = nameof(Of);
    public const string Results = nameof(Results);
    public const string Previous = nameof(Previous);
    public const string Next = nameof(Next);

    // Modals & Confirmations
    public const string AreYouSure = nameof(AreYouSure);
    public const string ConfirmDeleteText = nameof(ConfirmDeleteText);
    public const string ConfirmDeletePermanentlyText = nameof(ConfirmDeletePermanentlyText);
    public const string YesDeleteIt = nameof(YesDeleteIt);
    public const string DeletePermanentlyButton = nameof(DeletePermanentlyButton);

    // Admin Blog
    public const string BlogPosts = nameof(BlogPosts);
    public const string DeletedPosts = nameof(DeletedPosts);
    public const string SortByCreatedDate = nameof(SortByCreatedDate);
    public const string SortByTitle = nameof(SortByTitle);
    public const string SortByPublishDate = nameof(SortByPublishDate);
    public const string BlogPost = nameof(BlogPost);
    public const string CoverImage = nameof(CoverImage);
    public const string PublishedOnDate = nameof(PublishedOnDate);
    public const string Draft = nameof(Draft);
    public const string Published = nameof(Published);
    public const string PublicationDate = nameof(PublicationDate);
    public const string PublishDateInfo = nameof(PublishDateInfo);
    public const string IsPublished = nameof(IsPublished);
    public const string RemoveImage = nameof(RemoveImage);
    public const string ChooseFile = nameof(ChooseFile);
    public const string NoFileChosen = nameof(NoFileChosen);
    public const string CoverImageInfo = nameof(CoverImageInfo);
    public const string NoCoverImage = nameof(NoCoverImage);
    public const string CreateBlogPost = nameof(CreateBlogPost);
    public const string UpdateBlogPost = nameof(UpdateBlogPost);
    public const string BlogPostDetails = nameof(BlogPostDetails);
    public const string PostSettings = nameof(PostSettings);
    public const string Perex = nameof(Perex);
    public const string NoHtmlContent = nameof(NoHtmlContent);
    public const string AdditionalInfo = nameof(AdditionalInfo);
    public const string PlainText = nameof(PlainText);
    public const string NoPlainText = nameof(NoPlainText);
    public const string Slug = nameof(Slug);
    public const string DeletedOnDate = nameof(DeletedOnDate);
    public const string DeletionDate = nameof(DeletionDate);

    // Admin Contact Messages
    public const string ContactMessages = nameof(ContactMessages);
    public const string DeletedMessages = nameof(DeletedMessages);
    public const string SortByDate = nameof(SortByDate);
    public const string SortByStatus = nameof(SortByStatus);
    public const string Sender = nameof(Sender);
    public const string NoSubject = nameof(NoSubject);
    public const string Pending = nameof(Pending);
    public const string Handled = nameof(Handled);
    public const string Failed = nameof(Failed);
    public const string Handle = nameof(Handle);
    public const string MarkHandled = nameof(MarkHandled);
    public const string ConfirmDeleteMessageText = nameof(ConfirmDeleteMessageText);
    public const string ConfirmDeletePermanentlyMessageText = nameof(ConfirmDeletePermanentlyMessageText);
    public const string ContactMessageDetails = nameof(ContactMessageDetails);
    public const string MessageDetails = nameof(MessageDetails);
    public const string SenderContact = nameof(SenderContact);
    public const string Status = nameof(Status);
    public const string Date = nameof(Date);
    public const string ClientIp = nameof(ClientIp);
    public const string SentAtUtc = nameof(SentAtUtc);
    public const string NotRecorded = nameof(NotRecorded);
    public const string UserAgent = nameof(UserAgent);
    public const string FailureReason = nameof(FailureReason);
    public const string DeletedMessage = nameof(DeletedMessage);
    public const string MessageDeletedOn = nameof(MessageDeletedOn);
    public const string RestoreMessage = nameof(RestoreMessage);

    // Admin Content Pages
    public const string Pages = nameof(Pages);
    public const string DeletedPages = nameof(DeletedPages);
    public const string SortByOrder = nameof(SortByOrder);
    public const string Page = nameof(Page);
    public const string Locations = nameof(Locations);
    public const string Order = nameof(Order);
    public const string HomePage = nameof(HomePage);
    public const string TopNavigation = nameof(TopNavigation);
    public const string Footer = nameof(Footer);
    public const string PageDetails = nameof(PageDetails);
    public const string NoLocationsSelected = nameof(NoLocationsSelected);
    public const string NotSet = nameof(NotSet);
    public const string CreatePage = nameof(CreatePage);
    public const string EditPage = nameof(EditPage);
    public const string Settings = nameof(Settings);

    // Admin Media
    public const string MediaLibrary = nameof(MediaLibrary);
    public const string UploadMedia = nameof(UploadMedia);
    public const string DeletedAssets = nameof(DeletedAssets);
    public const string SortByUploadDate = nameof(SortByUploadDate);
    public const string SortByFilename = nameof(SortByFilename);
    public const string SortBySize = nameof(SortBySize);
    public const string Preview = nameof(Preview);
    public const string FileName = nameof(FileName);
    public const string Dimensions = nameof(Dimensions);
    public const string Size = nameof(Size);
    public const string View = nameof(View);
    public const string MediaDetails = nameof(MediaDetails);
    public const string AssetPreview = nameof(AssetPreview);
    public const string OriginalFileName = nameof(OriginalFileName);
    public const string ContentType = nameof(ContentType);
    public const string FileSize = nameof(FileSize);
    public const string Active = nameof(Active);
    public const string UploadedAtUtc = nameof(UploadedAtUtc);
    public const string Uploaded = nameof(Uploaded);
    public const string PublicPath = nameof(PublicPath);
    public const string EditMedia = nameof(EditMedia);
    public const string TitleInfo = nameof(TitleInfo);
    public const string AltTextInfo = nameof(AltTextInfo);
    public const string ClickToUpload = nameof(ClickToUpload);
    public const string MaxFileSizeInfo = nameof(MaxFileSizeInfo);

    // Quill RTE Tooltips
    public const string RteBold = nameof(RteBold);
    public const string RteItalic = nameof(RteItalic);
    public const string RteUnderline = nameof(RteUnderline);
    public const string RteStrikethrough = nameof(RteStrikethrough);
    public const string RteTextColor = nameof(RteTextColor);
    public const string RteBackgroundColor = nameof(RteBackgroundColor);
    public const string RteSubscript = nameof(RteSubscript);
    public const string RteSuperscript = nameof(RteSuperscript);
    public const string RteHeader1 = nameof(RteHeader1);
    public const string RteHeader2 = nameof(RteHeader2);
    public const string RteHeader3 = nameof(RteHeader3);
    public const string RteHeader4 = nameof(RteHeader4);
    public const string RteBlockquote = nameof(RteBlockquote);
    public const string RteCodeBlock = nameof(RteCodeBlock);
    public const string RteListOrdered = nameof(RteListOrdered);
    public const string RteListBullet = nameof(RteListBullet);
    public const string RteIndentDecrease = nameof(RteIndentDecrease);
    public const string RteIndentIncrease = nameof(RteIndentIncrease);
    public const string RteDirection = nameof(RteDirection);
    public const string RteAlign = nameof(RteAlign);
    public const string RteLink = nameof(RteLink);
    public const string RteImage = nameof(RteImage);
    public const string RteVideo = nameof(RteVideo);
    public const string RteFormula = nameof(RteFormula);
    public const string RteTable = nameof(RteTable);
    public const string RteRows = nameof(RteRows);
    public const string RteColumns = nameof(RteColumns);
    public const string RteInsert = nameof(RteInsert);
    public const string RteClean = nameof(RteClean);
    public const string RteFont = nameof(RteFont);
    public const string RteSize = nameof(RteSize);
    public const string RteCustomColor = nameof(RteCustomColor);
}


